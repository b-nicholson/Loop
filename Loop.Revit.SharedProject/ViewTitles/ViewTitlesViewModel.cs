using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities;
using Utilities.Units;
using Visibility = System.Windows.Visibility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;
using System.Windows.Controls;
using Loop.Revit.Utilities.Units;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf;
using Loop.Revit.Utilities.Wpf.DataGridUtils;
using Loop.Revit.Utilities.Wpf.WindowServices;
using MaterialDesignThemes.Wpf;
using Loop.Revit.Utilities.Wpf.SmallDialog;
using Loop.Revit.ViewTitles.Helpers;
using Loop.Revit.Utilities.Wpf.OutputListDialog;
using System.Windows.Controls.Primitives;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private readonly IWindowService _windowService;
        private readonly ErrorsViewModel _errorsViewModel;
        private readonly ViewTitlesModel _model;

        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();


        #region Command Properties
        public AsyncRelayCommand<Window> Run { get; set; }
        public RelayCommand<Window> CopyText { get; set; }
        public RelayCommand<Window> SaveUnits { get; set; }
        public RelayCommand Cancel { get; set; }
        public RelayCommand Test { get; set; }
        public RelayCommand Close { get; set; }
        #endregion

        #region Progress Bar Properties

        private int _currentProgress;
        public int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                SetProperty(ref _currentProgress, value);
                CheckProgressBarVisibility();
            }
        }

        private int _maxProgressValue;
        public int MaxProgressValue
        {
            get => _maxProgressValue;
            set => SetProperty(ref _maxProgressValue, value);
        }

        private Visibility _progressVisibility = Visibility.Collapsed;
        public Visibility ProgressVisibility
        {
            get => _progressVisibility;
            set => SetProperty(ref _progressVisibility, value);
        }
        #endregion


        #region DataGrid Properties
        //Data to bind to DataGrid
        private ObservableCollection<SheetWrapper> _sheets;
        public ObservableCollection<SheetWrapper> Sheets
        {
            get => _sheets;
            set => SetProperty(ref _sheets, value);
        }

        //ICollectionView so we can search using Filter methods
        public ICollectionView SheetView { get; set; }

        private int _selectedSheetCount;
        public int SelectedSheetCount
        {
            get => _selectedSheetCount;
            set => SetProperty(ref _selectedSheetCount, value);
        }
        
        //Multi select stuff
        private bool _isAllSheetsSelected;
        public bool IsAllSheetsSelected
        {
            get => _isAllSheetsSelected;
            set
            {
                if (_isAllSheetsSelected == value) return;
                SetProperty(ref _isAllSheetsSelected, value);
                SelectAllSheets(value);
            }
        }

        private void SelectAllSheets(bool select)
        {
            foreach (var sheet in Sheets)
            {
                sheet.IsSelected = select;
            }
            OnPropertyChanged(nameof(IsAllSheetsSelected));
        }


        // new attempts

        #region new attempts at datagrid checking
        private bool? _selectAllChecked = false;
        public bool? SelectAllChecked
        {
            get { return _selectAllChecked; }
            set
            {
                if (_selectAllChecked != value)
                {
                    SetProperty(ref _selectAllChecked, value);
                    UpdateItemSelections(value);
                }
            }
        }

        private void UpdateItemSelections(bool? selectAll)
        {
            if (selectAll == null) return;

            foreach (var item in Sheets)
            {
                item.IsSelected = selectAll == true;
            }
        }


        public void UpdateSelectAllChecked()
        {
            if (Sheets.All(item => item.IsSelected))
                SelectAllChecked = true;
            else if (Sheets.All(item => !item.IsSelected))
                SelectAllChecked = false;
            else
                SelectAllChecked = null; // Indeterminate state
        }
        #endregion




        // Search Stuff
        private string _textToFilter;
        public string TextToFilter
        {
            get => _textToFilter;
            set
            {
                SetProperty(ref _textToFilter, value);
                try
                {
                    SheetView.Filter = FilterByName;
                }
                catch (Exception)
                {
                    // this can throw exceptions for stupid reasons, just ignore
                }
            }
        }

        private bool _showCheckedOnly;
        public bool ShowCheckedItemsOnly
        {
            get => _showCheckedOnly;
            set
            {
                _showCheckedOnly = value;
                OnPropertyChanged(nameof(ShowCheckedItemsOnly));
                try
                {
                    SheetView.Filter = FilterByName;
                }
                catch (Exception)
                {
                    // this can throw exceptions for stupid reasons, just ignore
                }
            }

        }

        #endregion

        #region Units
        public ObservableCollection<RevitUnit> ComboBoxUnits => 
            new ObservableCollection<RevitUnit>(RevitUnitTypes.GetUnitsByType(SpecTypeId.Length));

        private RevitUnit _selectedUnit;
        public RevitUnit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;
                if (value != null)
                {
                    ConvertUnitsToInternal();
                    AccuracyOptions = new ObservableCollection<AccuracyWrapper>(value.Accuracy);
                    SetAccuracy();
                    OnPropertyChanged(nameof(Accuracy));
                }
            }
        }

        private AccuracyWrapper _accuracy;
        public AccuracyWrapper Accuracy
        {
            get => _accuracy;
            set
            {
                SetProperty(ref _accuracy, value);
                ConvertUnit(LengthInternalUnits);
            }
        }

        private ObservableCollection<AccuracyWrapper> _accuracyOptions;
        public ObservableCollection<AccuracyWrapper> AccuracyOptions
        {
            get => _accuracyOptions;
            set
            {
                _accuracyOptions?.Clear();
                SetProperty(ref _accuracyOptions, value);
            }
        }

        private Visibility _inputIsCalculation = Visibility.Collapsed;
        public Visibility InputIsCalculation
        {
            get => _inputIsCalculation;
            set => SetProperty(ref _inputIsCalculation, value);
        }

        private string _calculatedUnit;
        public string CalculatedUnit
        {
            get => _calculatedUnit;
            set
            {
                if (value == null) return;
                SetProperty(ref _calculatedUnit, value);
            }
        }

        private string _inputUnit;
        public string InputUnit
        {
            get => _inputUnit;
            set
            {
                SetProperty(ref _inputUnit, value);
                ConvertUnitsToInternal();
                InputIsCalculation = value.StartsWith("=") ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private double _lengthInternalUnits;
        public double LengthInternalUnits
        {
            get => _lengthInternalUnits;
            set
            {
                _model.ExtensionDistance = value;
                SetProperty(ref _lengthInternalUnits, value);
            }
        }

        public bool HasErrors => _errorsViewModel.HasErrors;

        public bool CanRun => !HasErrors;

        #endregion

        #region Sheet Parameters

        private ObservableCollection<PropertyWrapper> _sheetParameters;
        public ObservableCollection<PropertyWrapper> SheetParameters
        {
            get => _sheetParameters;
            set => SetProperty(ref _sheetParameters, value);
        }

        private PropertyWrapper _selectedPropWrapper;
        public PropertyWrapper SelectedPropertyWrapper
        {
            get => _selectedPropWrapper;
            set
            {
                SetProperty(ref _selectedPropWrapper, value);
                try
                {
                    SheetView.Filter = FilterByName;
                }
                catch (Exception)
                {
                    // this can throw exceptions for stupid reasons, just ignore
                }
            }
        }
        #endregion

        public ViewTitlesViewModel(ViewTitlesModel model, IWindowService windowService)
        {
            _model = model;
            _windowService = windowService;

            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;

            #region Datagrid stuff
            Sheets = new ObservableCollection<SheetWrapper>(_model.CollectSheets().OrderBy(o => o.SheetNumber).ToList());

            //TODO move to a method inside model
            var activeView = _model.ActiveView;
            if (activeView.ViewType == ViewType.DrawingSheet)
            {
                foreach (var sheet in Sheets)
                {
                    var id = sheet.Id;
                    if (id == activeView.Id)
                    {
                        sheet.IsSelected = true;
                        break;
                    }

                }
            }

            //put the obs. coll. into a ICollectionView so we can filter it
            SheetView = CollectionViewSource.GetDefaultView(Sheets);

            // create a filter for the DataGrid
            SheetView.Filter = FilterByName;
            #endregion

            // Set combobox unit to the unit used in the model
            SelectedUnit = ComboBoxUnits.FirstOrDefault(u => u.UnitTypeId == _model.CollectUnitsTypeId());

            //Set Accuracy units
            SetAccuracy();

            //Load ExtensibleStorage
            _model.LoadDataStorage();
            var convertedUnit = _model.FormatUnits(_model.ExtensionDistance, SelectedUnit, Accuracy.Value);
            InputUnit = convertedUnit;

            #region Sheet Properties for Filtering
            var allSheetProperties = new ObservableCollection<PropertyInfo>(typeof(SheetWrapper).GetProperties()
                .Where(prop => prop.PropertyType == typeof(string)).ToList());

            var propertyWrappers = new ObservableCollection<PropertyWrapper>
            {
                new PropertyWrapper("ALL", allSheetProperties)
            };

            foreach (var prop in allSheetProperties)
            {
                var oc = new ObservableCollection<PropertyInfo> {prop};
                propertyWrappers.Add(new PropertyWrapper(prop.Name, oc));
            }

            SheetParameters = propertyWrappers;
            SelectedPropertyWrapper = propertyWrappers[0];
            #endregion

            //TODO check if async actually does anything
            Run = new AsyncRelayCommand<Window>(OnRun);
            CopyText = new RelayCommand<Window>(OnCopyText);
            SaveUnits = new RelayCommand<Window>(OnSaveSettings);
            Cancel = new RelayCommand(OnCancel);
            Test = new RelayCommand(OnTest);
            Close = new RelayCommand(OnClose);
            

            WeakReferenceMessenger.Default.Register<ViewTitlesViewModel, ProgressResultsMessage>(this, (r, m) => r.OnProgressUpdate(m));
            WeakReferenceMessenger.Default.Register<ViewTitlesViewModel, OperationResultMessage>(this, (r, m) => r.OnOperationResult(m));
            WeakReferenceMessenger.Default.Register<ViewTitlesViewModel, NonEditableViewportsMessage>(this, (r, m) => r.OnNonEditableViewports(m));

            LoadSettings();
        }

        private void OnClose()
        {
            _windowService.CloseWindow();
        }

        private void OnTest()
        {
            var testSheets = _model.CollectSheets().OrderBy(o => o.SheetNumber).ToList();
            var ids = testSheets.SelectMany(sheet => sheet.ViewportIds).ToList();

            var viewports = new FilteredElementCollector(_model.Doc, ids)
                .OfCategory(BuiltInCategory.OST_Viewports)
                .Cast<Viewport>();

            var testlist = new List<ViewportWrapper>();

            var rdm = new Random();

            foreach (var vp in viewports)
            {
                testlist.Add(new ViewportWrapper(vp, ""));

            }
            
            var columns = new ObservableCollection<DataGridColumnModel>();
            columns.Add(new DataGridButtonColumnModel
            {
                Header = "Find\nView",
                Content = PackIconKind.Search,
                CommandParameterPath = "Id",
                Width = new DataGridLength(1, DataGridLengthUnitType.Auto)
                // Set other properties as needed
            });
            columns.Add(new DataGridColumnModel { Header = "Owner", BindingPath = "Owner", BindingMode = BindingMode.OneWay, Width = new DataGridLength(100, DataGridLengthUnitType.SizeToCells) });
            columns.Add(new DataGridColumnModel { Header = "Sheet Number",
                BindingPath = "SheetNumber", 
                BindingMode = BindingMode.OneWay,
                HeaderTextAlignment = TextAlignment.Center, 
                Width = new DataGridLength(1, DataGridLengthUnitType.SizeToHeader) });
           
            columns.Add(new DataGridColumnModel { Header = "Sheet Name", BindingPath = "SheetName", BindingMode = BindingMode.OneWay, Width = new DataGridLength(100, DataGridLengthUnitType.SizeToCells) });
            columns.Add(new DataGridColumnModel { Header = "View Name", BindingPath = "ViewName", BindingMode = BindingMode.OneWay, Width = new DataGridLength(100, DataGridLengthUnitType.Auto) });
            columns.Add(new DataGridColumnModel { Header = "Title On Sheet", BindingPath = "TitleOnSheet", BindingMode = BindingMode.OneWay, Width = new DataGridLength(100, DataGridLengthUnitType.Star) });
       

            var title = "Viewports Unable To Be Edited:";
            OutputListDialog.Create(testlist, columns, title: title, modeless: true, uiDoc:_model.UiDoc);

    
        }

        private void OnNonEditableViewports(NonEditableViewportsMessage obj)
        {

        }

        private void OnCancel()
        {
            WeakReferenceMessenger.Default.Send(new CancelMessage(true));
        }

        private void LoadSettings()
        {
            var darkmode = GlobalSettings.Settings.IsDarkModeTheme;
            _windowService.ToggleDarkMode(darkmode);
            var colour = GlobalSettings.Settings.PrimaryThemeColor;
            var theme = _windowService.GetMaterialDesignTheme();
            theme.SetPrimaryColor(colour);
            _windowService.SetMaterialDesignTheme(theme);
        }

        private void OnOperationResult(OperationResultMessage obj)
        {

            //TODO: Change the success method to adapt to the OperationResultMessage
            if (obj.Result.Success == true)
            {
                switch (obj.Result.OperationType)
                {
                    case nameof(ViewTitlesRequestHandler.CreateDataStorage):
                        MessageQueue.Enqueue(content: "✅ Settings Saved");
                        MessageQueue.Enqueue(content: "Remember to Save/Sync to keep changes");
                        break;
                    case nameof(ViewTitlesRequestHandler.AdjustViewTitles):
                        MessageQueue.Enqueue(content: obj.Result.Message);
                        break;
                }
            }
            else
            {
                MessageQueue.Enqueue("❎ Settings Not Saved",
                    actionContent: "Show",
                    actionHandler: new Action<string>(SnackBarErrorAction),
                    actionArgument: obj.Result.Message);
            }
          
        }

        private void OnProgressUpdate(ProgressResultsMessage obj)
        {
            CurrentProgress = obj.CurrentSheetProgress;
            MaxProgressValue = obj.TotalSheetCount;
        }

        private void SetAccuracy()
        {
            try
            {
                Accuracy = AccuracyOptions[_model.FindClosestUnitAccuracyIndex(SelectedUnit)];
            }
            catch (Exception)
            {
                //do nothing
            }

        }

        private bool FilterByName(object obj)
        {
            var sheetInfo = (SheetWrapper)obj;
            if (TextToFilter == null)
            {
                if (ShowCheckedItemsOnly) return sheetInfo.IsSelected;
                else return true;
            }

            var selection = SelectedPropertyWrapper;
            var parametersToTarget = selection.Value;

            var isFound = false;
            foreach (var param in parametersToTarget)
            {
                var paramValue = sheetInfo.GetType().GetProperty(param.Name).GetValue(sheetInfo, null).ToString();
                if (paramValue.IndexOf(TextToFilter, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    isFound = true;
                    break;
                }
            }

            if (ShowCheckedItemsOnly)
            {
                return sheetInfo != null && isFound && sheetInfo.IsSelected;
            }
            else return sheetInfo != null && isFound;
        }

        private async Task OnRun(Window win)
        {
            var selected = Sheets.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                return;
            }
            await Task.Run(() => _model.ChangeTitleLength(selected));
        }

        private void OnCopyText(Window win)
        {
            InputUnit = CalculatedUnit;
            OnPropertyChanged(nameof(InputUnit));
        }

        private void OnSaveSettings(Window win)
        {
            _model.CreateDataStorage(LengthInternalUnits);
        }

        #region INotifyErrorInfo
        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsViewModel.GetErrors(propertyName);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void ErrorsViewModel_ErrorsChanged(object sender, DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
            OnPropertyChanged(nameof(CanRun));
        }
        #endregion

        private void ConvertUnitsToInternal()
        {
            if (_inputUnit != null)
            {
                var validationResults = _model.TryParseTextToInternalUnits(_inputUnit, SelectedUnit.Unit);
                var validationNumber = validationResults.Item1;
                var errorMessages = validationResults.Item2;

                _errorsViewModel.ClearErrors(nameof(InputUnit));

                if (!String.IsNullOrEmpty(errorMessages))
                {
                    _errorsViewModel.AddError(nameof(InputUnit), errorMessages);
                }

                LengthInternalUnits = validationNumber;


                ConvertUnit(validationNumber);


            }
        }

        private void ConvertUnit(double numberToConvert)
        {
            if (Accuracy == null) return;
            var accuracyValue = Accuracy.Value;
            var convertedUnit = _model.FormatUnits(numberToConvert, SelectedUnit, accuracyValue);
            CalculatedUnit = convertedUnit;
        }

        private void CheckProgressBarVisibility()
        {
            if (CurrentProgress < MaxProgressValue)
            {
                ProgressVisibility = Visibility.Visible;
            }
            else
            {
                ProgressVisibility = Visibility.Collapsed;
            }
        }

        private void SnackBarErrorAction(string message)
        {
            var win = _windowService.GetWindow();
            var theme = _windowService.GetMaterialDesignTheme();
            SmallDialog.Create(
                title: "Error!",
                message: message,
                button1: new SdButton("OK", SmallDialogResults.Yes),
                theme: theme,
                owner: win
            );

        }

    }
}

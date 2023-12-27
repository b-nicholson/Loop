using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities;
using MaterialDesignThemes.Wpf;
using Utilities.Units;
using Visibility = System.Windows.Visibility;
using Loop.Revit.Utilities.Wpf;

using CommunityToolkit.Mvvm.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Threading.Tasks;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesViewModel : ObservableObject, INotifyDataErrorInfo
    {
        private readonly ErrorsViewModel _errorsViewModel;

        private readonly ViewTitlesModel _model;

        public AsyncRelayCommand<Window> Run { get; set; }
        public RelayCommand<Window> CopyText { get; set; }

        public RelayCommand<Window> SaveUnits { get; set; }

        private int _currentProgress;

        public int CurrentProgress
        {
            get => _currentProgress;

            set => SetProperty(ref _currentProgress, value);
        }

        private int _maxProgressValue;
        public int MaxProgressValue
        {
            get => _maxProgressValue;
            set => SetProperty(ref _maxProgressValue, value);
        }

        private bool _isDarkMode;

        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                SetProperty(ref _isDarkMode, value);
                OnPropertyChanged(nameof(IsDarkMode));
            } 
        }

        public RelayCommand ToggleThemeCommand { get; }



        #region DataGrid Stuff
        //Data to bind to DataGrid
        private ObservableCollection<SheetWrapper> _sheets;
        public ObservableCollection<SheetWrapper> Sheets
        {
            get => _sheets;
            set { _sheets = value;
                OnPropertyChanged(nameof(Sheets));
            }
        }

        //ICollectionView so we can search using Filter methods
        public ICollectionView SheetView { get; set; }

        private int _selectedSheetCount;

        public int SelectedSheetCount
        {
            get => _selectedSheetCount;
            set
            {
                _selectedSheetCount = value;
                OnPropertyChanged(nameof(SelectedSheetCount));
            }
        }




        //Multi select stuff
        private bool _isAllSheetsSelected;
        public bool IsAllSheetsSelected
        {
            get => _isAllSheetsSelected;
            set
            {
                if (_isAllSheetsSelected == value) return;
                _isAllSheetsSelected = value;
                OnPropertyChanged(nameof(IsAllSheetsSelected));
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
                    _selectAllChecked = value;
                    OnPropertyChanged(nameof(SelectAllChecked));
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
                _textToFilter = value;
                OnPropertyChanged(nameof(TextToFilter));
                try
                {
                    SheetView.Filter = FilterByName;
                }
                catch (Exception e)
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
                catch (Exception e)
                {
                    // this can throw exceptions for stupid reasons, just ignore
                }
            }

        }

        #endregion


        #region Units
        public ObservableCollection<RevitUnit> ComboBoxUnits => new ObservableCollection<RevitUnit>(RevitUnitTypes.GetUnitsByType(SpecTypeId.Length));


        private RevitUnit _selectedUnit;

        public RevitUnit SelectedUnit
        {
            get => _selectedUnit;
            set
            {
                _selectedUnit = value;

                //InputUnit = _inputUnit;

                if (value != null)
                {
                    ConvertUnitsToInternal();
                    AccuracyOptions = new ObservableCollection<double>(value.Accuracy);


                    SetAccuracy();
                    OnPropertyChanged(nameof(Accuracy));


                }


            }
        }

        private double _accuracy;

        public double Accuracy
        {
            get => _accuracy;
            set
            {
                _accuracy = value;
                OnPropertyChanged(nameof(Accuracy));
                ConvertUnit(lengthInternalUnits);


            }
        }

        private ObservableCollection<double> _accuracyOptions;

        public ObservableCollection<double> AccuracyOptions
        {
            get => _accuracyOptions;
            set
            {
                _accuracyOptions?.Clear();
                _accuracyOptions = value;
                OnPropertyChanged(nameof(AccuracyOptions));
            }
        }


        //private string _errorMessage;
        //public string ErrorMessage
        //{
        //    get => _errorMessage;
        //    set
        //    {
        //        _errorMessage = value;
        //    }
        //}

        //private readonly WpfRevitUnitValidator _validator = new WpfRevitUnitValidator();



        // for hiding and showing the units calculator
        private Visibility _inputIsCalculation = Visibility.Collapsed;

        public Visibility InputIsCalculation
        {
            get => _inputIsCalculation;
            set
            {
                _inputIsCalculation = value;
                OnPropertyChanged(nameof(InputIsCalculation));
            }
        }


        private string _calculatedUnit;

        public string CalculatedUnit
        {
            get => _calculatedUnit;
            set
            {
                if (value != null)
                {
                    _calculatedUnit = value;
                    OnPropertyChanged(nameof(CalculatedUnit));
                }
            }
        }



        private string _inputUnit;
        public string InputUnit
        {
            get => _inputUnit;
            set
            {
                _inputUnit = value;
                ConvertUnitsToInternal();

                if (value.StartsWith("="))
                {
                    InputIsCalculation = Visibility.Visible;
                }
                else
                    InputIsCalculation = Visibility.Collapsed;

                // use ValidateProperty if using fluent validation
                //ValidateProperty(nameof(InputUnit));

            }
        }

        private double _lengthInternalUnits;

        public double lengthInternalUnits
        {
            get => _lengthInternalUnits;
            set
            {
                _lengthInternalUnits = value;
                _model.ExtensionDistance = value;
                OnPropertyChanged(nameof(lengthInternalUnits));
            }
        }

        public bool HasErrors => _errorsViewModel.HasErrors;

        public bool CanRun => !HasErrors;

        #endregion

        #region Sheet Parameters
        public ObservableCollection<PropertyInfo> SheetParams => new ObservableCollection<PropertyInfo>(
            typeof(SheetWrapper).GetProperties().Where(prop => prop.PropertyType == typeof(string)).ToList());

        //var stuff3 = typeof(SheetWrapper).GetProperties().Where(prop => prop.PropertyType == typeof(string)).ToList();

        private ObservableCollection<PropertyWrapper> _sheetParameters;
        public ObservableCollection<PropertyWrapper> SheetParameters
        {
            get => _sheetParameters;
            set
            {
                _sheetParameters = value;
                OnPropertyChanged(nameof(SheetParameters));
            }
        }

        private PropertyWrapper _selectedPropWrapper;
        public PropertyWrapper SelectedPropertyWrapper
        {
            get { return _selectedPropWrapper; }
            set
            {
                _selectedPropWrapper = value;
                OnPropertyChanged(nameof(SelectedPropertyWrapper));
                try
                {
                    SheetView.Filter = FilterByName;
                }
                catch (Exception e)
                {
                    // this can throw exceptions for stupid reasons, just ignore
                }
            }
        }
        #endregion

        #region Example Image stuff
        //property to control the example image in the xaml window 
        private BitmapImage _imageExample;
        public BitmapImage ImageExample
        {
            get => _imageExample;
            set
            {
                _imageExample = value;
                OnPropertyChanged(nameof(ImageExample));
            }
        }
        #endregion




        public ViewTitlesViewModel(ViewTitlesModel model)
        {
            _model = model;

            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;


            #region Datagrid stuff
            Sheets = new ObservableCollection<SheetWrapper>(_model.CollectSheets().OrderBy(o => o.SheetNumber).ToList());

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


            //Set image in window
            ImageExample = ImageUtils.LoadImage(Assembly.GetExecutingAssembly(), "viewTitles.example.png");

            //Set Accuracy units
            SetAccuracy();

            //Load ExtensibleStorage
            _model.LoadDataStorage();
            var things = _model.ExtensionDistance;
            var things2 = SelectedUnit;
            var things3 = Accuracy;
            var convertedUnit = _model.FormatUnits(_model.ExtensionDistance, SelectedUnit, Accuracy);
            InputUnit = convertedUnit;


            #region Sheet Properties for Filtering
            var allSheetProperties = new ObservableCollection<PropertyInfo>(typeof(SheetWrapper).GetProperties()
                .Where(prop => prop.PropertyType == typeof(string)).ToList());

            var propertyWrappers = new ObservableCollection<PropertyWrapper>();
            propertyWrappers.Add(new PropertyWrapper("ALL", allSheetProperties));

            foreach (var prop in allSheetProperties)
            {
                var oc = new ObservableCollection<PropertyInfo>();
                oc.Add(prop);

                propertyWrappers.Add(new PropertyWrapper(prop.Name, oc));
            }

            SheetParameters = propertyWrappers;
            SelectedPropertyWrapper = propertyWrappers[0];
            #endregion

 

            Run = new AsyncRelayCommand<Window>(OnRun);
            CopyText = new RelayCommand<Window>(OnCopyText);
            SaveUnits = new RelayCommand<Window>(OnSaveSettings);
            ToggleThemeCommand = new RelayCommand(() => IsDarkMode = !IsDarkMode);

            WeakReferenceMessenger.Default.Register<ViewTitlesViewModel, ProgressResultsMessage>(this, (r, m) => r.OnProgressUpdate(m));
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
            catch (Exception e)
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
            _model.CreateDataStorage(lengthInternalUnits);
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

                lengthInternalUnits = validationNumber;


                ConvertUnit(validationNumber);


            }
        }

        private void ConvertUnit(double numberToConvert)
        {
            var convertedUnit = _model.FormatUnits(numberToConvert, SelectedUnit, Accuracy);
            CalculatedUnit = convertedUnit;
        }

        //fluent validation
        private void ValidateProperty(string propertyName)
        {
            // this is commented out becuase i'm not actually using fluent validation for this anymore. saving for future use/reference.


            _errorsViewModel.ClearErrors(propertyName);
            //var result = _validator.Validate(UserUnit);
            //if (result.Errors.Any())
            //{
            //    string firstErrorMessage = result.Errors.First().ErrorMessage;
            //    _errorsViewModel.AddError(propertyName, firstErrorMessage);
            //}
        }


    }
}

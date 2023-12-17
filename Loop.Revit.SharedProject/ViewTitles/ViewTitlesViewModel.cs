using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Loop.Revit.Utilities;
using Utilities;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        public ViewTitlesModel Model { get; set; }

        private readonly ErrorsViewModel _errorsViewModel;

        public RelayCommand<Window> Run { get; set; }


        #region DataGrid Stuff
        //Data to bind to DataGrid
        private ObservableCollection<SheetWrapper> _sheets;
        public ObservableCollection<SheetWrapper> Sheets
        {
            get => _sheets;
            set { _sheets = value; RaisePropertyChanged(() => Sheets); }
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
                RaisePropertyChanged(nameof(SelectedSheetCount));
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
                RaisePropertyChanged(nameof(IsAllSheetsSelected));
                SelectAllSheets(value);
            }
        }

        private void SelectAllSheets(bool select)
        {
            foreach (var sheet in Sheets)
            {
                sheet.IsSelected = select;
            }
            RaisePropertyChanged(nameof(IsAllSheetsSelected));
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
                    RaisePropertyChanged(nameof(SelectAllChecked));
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
                RaisePropertyChanged(nameof(TextToFilter));
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
        public RevitUnit SelectedUnit { get; set; }



        private WpfUnit _userUnit;
        public WpfUnit UserUnit
        {
            get => _userUnit;
            set
            {
                _userUnit = value;
                RaisePropertyChanged(nameof(UserUnit));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
            }
        }

        private readonly WpfRevitUnitValidator _validator = new WpfRevitUnitValidator();

        private string _inputUnit;
        public string InputUnit
        {
            get => _inputUnit;
            set
            {
                _inputUnit = value;
                UserUnit = new WpfUnit(value, SelectedUnit);
                ValidateProperty(nameof(InputUnit));



                ////Validate userinput for Units

                //var validator = new WpfRevitUnitValidator();
                //var results = validator.Validate(UserUnit);

                //var errors = new List<string>();

                //if (results.IsValid == false)
                //{
                //    errors.AddRange(results.Errors.Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}"));
                //}

                
                
                ////ErrorMessage = results.Errors[0].ErrorMessage;


                ////Inotifypropchanged stuff, replace w fluent

                //_errorsViewModel.ClearErrors(nameof(InputUnit));

                //if (_inputUnit.Length > 3)
                //{
                //    _errorsViewModel.AddError(nameof(InputUnit), "Invalid Unit");
                //}




            }
        }

        public bool HasErrors => _errorsViewModel.HasErrors;

        public bool CanRun => !HasErrors;

        #endregion


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
                RaisePropertyChanged(nameof(SheetParameters));
            }
        }

        private PropertyWrapper _selectedPropWrapper;

        public PropertyWrapper SelectedPropertyWrapper
        {
            get { return _selectedPropWrapper; }
            set
            {
                _selectedPropWrapper = SelectedPropertyWrapper;
                RaisePropertyChanged(nameof(SelectedPropertyWrapper));
            }
        }




        #region Example Image stuff
        //property to control the example image in the xaml window 
        private BitmapImage _imageExample;
        public BitmapImage ImageExample
        {
            get => _imageExample;
            set
            {
                _imageExample = value;
                RaisePropertyChanged(nameof(ImageExample));
            }
        }
        #endregion




        public ViewTitlesViewModel(ViewTitlesModel model)
        {
            Model = model;

            _errorsViewModel = new ErrorsViewModel();
            _errorsViewModel.ErrorsChanged += ErrorsViewModel_ErrorsChanged;


            #region Datagrid stuff
            Sheets = new ObservableCollection<SheetWrapper>(Model.CollectSheets().OrderBy(o => o.SheetNumber).ToList());
            //put the obs. coll. into a ICollectionView so we can filter it
            SheetView = CollectionViewSource.GetDefaultView(Sheets);

            // create a filter for the DataGrid
            SheetView.Filter = FilterByName;
            #endregion

            // Set combobox unit to the unit used in the model
            SelectedUnit = ComboBoxUnits.FirstOrDefault(u => u.UnitTypeId == Model.CollectUnits());


            //Set image in window
            ImageExample = ImageUtils.LoadImage(Assembly.GetExecutingAssembly(), "viewTitles.example.png");


        




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

            //foreach (var sheet in Sheets)
            //{
            //    sheet.PropertyChanged += (sender, args) =>
            //    {
            //        if (args.PropertyName == nameof(SheetWrapper.IsSelected))

            //            RaisePropertyChanged(nameof(IsAllSheetsSelected));
            //    };
            //}

            Run = new RelayCommand<Window>(OnRun);
        }

        private bool FilterByName(object obj)
        {
            if (TextToFilter == null) return true;
            var sheetInfo = (SheetWrapper)obj;

            var para = SheetParameters;
            var selection = SelectedPropertyWrapper;







            var isFound = false;
            foreach (var p in para)
            {
                var val = p.Value;
                foreach (var param in val)
                {
                    var stuff = sheetInfo.GetType().GetProperty(param.Name).GetValue(sheetInfo, null);
                }

            }


            var textContainsCaseInsensitive = sheetInfo.SheetName.IndexOf(TextToFilter, StringComparison.OrdinalIgnoreCase) >= 0;

            return sheetInfo != null && textContainsCaseInsensitive;
        }




        private void OnRun(Window win)
        {
            var selected = Sheets.Where(x => x.IsSelected).ToList();
            if (selected.Count == 0)
            {
                return;
            }
            Model.ChangeTitleLength(selected);
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
            RaisePropertyChanged(nameof(CanRun));
        }
        #endregion

         //fluent validation
         private void ValidateProperty(string propertyName)
         {
             _errorsViewModel.ClearErrors(propertyName);
            var result = _validator.Validate(UserUnit);
            if (result.Errors.Any())
            {
                string firstErrorMessage = result.Errors.First().ErrorMessage;
                _errorsViewModel.AddError(propertyName, firstErrorMessage);
            }
         }


    }
}

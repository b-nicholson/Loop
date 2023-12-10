using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Loop.Revit.Utilities;
using Utilities;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesViewModel : ViewModelBase
    {
        public ViewTitlesModel Model { get; set; }

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

        // Search Stuff
        private string _textToFilter;
        public string TextToFilter
        {
            get { return _textToFilter; }
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



        public ObservableCollection<RevitUnit> ComboBoxUnits => new ObservableCollection<RevitUnit>(RevitUnitTypes.GetUnitsByType(SpecTypeId.Length));
        public RevitUnit SelectedUnit { get; set; }

        public ObservableCollection<PropertyInfo> SheetParams => new ObservableCollection<PropertyInfo>(
            typeof(SheetWrapper).GetProperties().Where(prop => prop.PropertyType == typeof(string)).ToList());

        //var stuff3 = typeof(SheetWrapper).GetProperties().Where(prop => prop.PropertyType == typeof(string)).ToList();

        //public BitmapImage ExampleImage =
        //    ImageUtils.LoadImage(Assembly.GetExecutingAssembly(), "viewTitles.example.png");

        private BitmapImage _imageExample;

        public BitmapImage ImageExample
        {
            get { return _imageExample; }
            set
            {
                _imageExample = value;
                RaisePropertyChanged(nameof(ImageExample));
            }
        }



        public ViewTitlesViewModel(ViewTitlesModel model)
        {
            Model = model;

            #region Datagrid stuff
            Sheets = new ObservableCollection<SheetWrapper>(Model.CollectSheets().OrderBy(o => o.SheetNumber).ToList());
            //put the obs. coll. into a ICollectionView so we can filter it
            SheetView = CollectionViewSource.GetDefaultView(Sheets);

            // create a filter for the DataGrid
            SheetView.Filter = FilterByName;
            #endregion

            // Set combobox unit to the unit used in the model
            SelectedUnit = ComboBoxUnits.FirstOrDefault(u => u.UnitTypeId == Model.CollectUnits());


            var imtg = ImageUtils.LoadImage(Assembly.GetExecutingAssembly(), "viewTitles.example.png");


            ImageExample = imtg;

            var a = Assembly.GetExecutingAssembly();

            var img = new BitmapImage();

            var resourceName = a.GetManifestResourceNames().FirstOrDefault(x => x.Contains("viewTitles.example.png"));
            var sttt = a.GetManifestResourceNames();
            var stream = a.GetManifestResourceStream(resourceName);

            var t = a.GetName().ToString();
            var e = a.Location;



            img.BeginInit();
            img.StreamSource = stream;
            img.EndInit();







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



    }
}

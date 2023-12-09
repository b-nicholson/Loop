using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Data;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Utilities;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesViewModel : ViewModelBase
    {
        public ViewTitlesModel Model { get; set; }

        public RelayCommand<Window> Run { get; set; }

        private ObservableCollection<SheetWrapper> _sheets;

        public ObservableCollection<SheetWrapper> Sheets
        {
            get => _sheets;
            set { _sheets = value; RaisePropertyChanged(() => Sheets); }
        }

        

        public ObservableCollection<RevitUnit> ComboBoxUnits => new ObservableCollection<RevitUnit>(RevitUnitTypes.GetUnitsByType(SpecTypeId.Length));


        public RevitUnit SelectedUnit { get; set; }

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
            foreach (var sheet in Sheets )
            {
                sheet.IsSelected = select;
            }
            RaisePropertyChanged(nameof(IsAllSheetsSelected));
        }

        private string _textToFilter;

        public string TextToFilter
        {
            get { return _textToFilter; }
            set
            {
                _textToFilter = value;
                RaisePropertyChanged(nameof(TextToFilter));
            }
        }



        public ViewTitlesViewModel(ViewTitlesModel model)
        {
            Model = model;
            Sheets = Model.CollectSheets();
            var sheet2 = CollectionViewSource.GetDefaultView(Sheets);
            SelectedUnit = ComboBoxUnits.FirstOrDefault(u => u.UnitTypeId == Model.CollectUnits());
            //Sheets.Filter = 


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

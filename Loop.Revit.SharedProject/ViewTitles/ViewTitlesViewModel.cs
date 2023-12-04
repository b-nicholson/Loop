using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesViewModel : ViewModelBase
    {
        public ViewTitlesModel Model { get; set; }

        public RelayCommand<Window> Run { get; set; }

        private ObservableCollection<SheetWrapper> _sheets;

        public ObservableCollection<SheetWrapper> Sheets
        {
            get { return _sheets; }
            set { _sheets = value; RaisePropertyChanged(() => Sheets); }
        }

        private bool _isAllSheetsSelected;


        public bool IsAllSheetsSelected
        {
            get { return _isAllSheetsSelected; }
            set
            {
                if (_isAllSheetsSelected != value)
                {
                    _isAllSheetsSelected = value;
                    RaisePropertyChanged(nameof(IsAllSheetsSelected));
                    SelectAllSheets(value);
                }
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

        public ViewTitlesViewModel(ViewTitlesModel model)
        {
            Model = model;
            Sheets = Model.CollectSheets();

            foreach (var sheet in Sheets)
            {
                sheet.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == nameof(SheetWrapper.IsSelected))

                        RaisePropertyChanged(nameof(IsAllSheetsSelected));
                };
            }

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

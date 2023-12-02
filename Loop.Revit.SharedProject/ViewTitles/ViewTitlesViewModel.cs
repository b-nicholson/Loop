using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public RelayCommand<Window> Close { get; set; }
        public RelayCommand<Window> Delete { get; set; }

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


            // Close = new RelayCommand<Window>(OnClose);
            // Delete = new RelayCommand<Window>(OnDelete);

            //  Messenger.Default.Register<SpatialObjectDeletedMessage>(this, OnSpatialElementDeletedMessage);
        }

        // private ObservableCollection<SpatialObjectWrapper> _spatialObjects;

        // public ObservableCollection<SpatialObjectWrapper> SpatialObjects
        //  {
        //     get { return _spatialObjects; }
        //    set { _spatialObjects = value; RaisePropertyChanged(() => SpatialObjects); }
        //  }
    }
}

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

        public bool? IsAllSheetsSelected
        {
            get
            {
                var selected = Sheets.Select(item => item.IsSelected).Distinct().ToList();
                return selected.Count == 1 ? selected.Single() : (bool?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    SelectAll(value.Value, Sheets);
                    RaisePropertyChanged(() => Sheets);


                }
            }
        }

        private static void SelectAll(bool select, ObservableCollection<SheetWrapper> sheets)
        {
            foreach (var sheet in sheets )
            {
                sheet.IsSelected = select;
            }
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
                        ;
                    // OnPropertyChanged(nameof(IsAllSheetsSelected));
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

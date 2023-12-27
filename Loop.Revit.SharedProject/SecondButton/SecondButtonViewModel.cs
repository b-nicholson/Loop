using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Loop.Revit.SecondButton
{
    public class SecondButtonViewModel : ObservableObject
    {
        public SecondButtonModel Model { get; set; }

        public RelayCommand<Window> Close { get; set; }
        public RelayCommand<Window> Delete { get; set; }

        private ObservableCollection<SpatialObjectWrapper> _spatialObjects;

        public ObservableCollection<SpatialObjectWrapper> SpatialObjects
        {
            get { return _spatialObjects;}
            set { _spatialObjects = value; OnPropertyChanged(nameof(SpatialObjects)); }
        }

        public SecondButtonViewModel(SecondButtonModel model)
        {
            Model = model;
            SpatialObjects = Model.CollectSpatialObjects();
            Close = new RelayCommand<Window>(OnClose);
            Delete = new RelayCommand<Window>(OnDelete);
        }

        private void OnClose(Window win)
        {
            win.Close();
        }

        private void OnDelete(Window win)
        {
            var selected = SpatialObjects.Where(x => x.IsSelected).ToList();
            Model.Delete(selected);
            win.Close();
        }


    }
}

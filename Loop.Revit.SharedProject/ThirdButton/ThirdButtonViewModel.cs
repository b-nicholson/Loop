using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;


namespace Loop.Revit.ThirdButton
{
    public class ThirdButtonViewModel : ObservableObject
    {
        public ThirdButtonModel Model { get; set; }

        public RelayCommand<Window> Close { get; set; }
        public RelayCommand<Window> Delete { get; set; }

        private ObservableCollection<SpatialObjectWrapper> _spatialObjects;

        public ObservableCollection<SpatialObjectWrapper> SpatialObjects
        {
            get { return _spatialObjects; }
            set { _spatialObjects = value; OnPropertyChanged(nameof(SpatialObjects)); }
        }

        public ThirdButtonViewModel(ThirdButtonModel model)
        {
            Model = model;
            SpatialObjects = Model.CollectSpatialObjects();
            Close = new RelayCommand<Window>(OnClose);
            Delete = new RelayCommand<Window>(OnDelete);
            

            WeakReferenceMessenger.Default.Register<ThirdButtonViewModel ,SpatialObjectDeletedMessage>(this,  (r, m) => r.OnSpatialElementDeletedMessage(m));
        }

        private void OnSpatialElementDeletedMessage(SpatialObjectDeletedMessage obj)
        {
            var spatialObjects = SpatialObjects.Where(x => !obj.Ids.Contains(x.Id));
            SpatialObjects = new ObservableCollection<SpatialObjectWrapper>(spatialObjects);
        }

        private void OnClose(Window win)
        {
            win.Close();
        }

        private void OnDelete(Window win)
        {
            var selected = SpatialObjects.Where(x => x.IsSelected).ToList();
            Model.Delete(selected);
        }


    }
}

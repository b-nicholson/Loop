using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace Loop.Revit.SecondButton
{
    public class SpatialObjectWrapper : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public double Area { get; set; }

        public ElementId Id { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; RaisePropertyChanged(nameof(IsSelected));}
        }

        public SpatialObjectWrapper(SpatialElement room)
        {
            Name = room.Name;
            Area = room.Area;
            Id = room.Id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

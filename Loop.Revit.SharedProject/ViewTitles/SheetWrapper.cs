using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Loop.Revit.ViewTitles
{
    public class SheetWrapper : INotifyPropertyChanged
    {
        public string SheetNumber { get; set; }
        public string SheetName { get; set; }
        public ElementId Id { get; set; }

        private bool _isSelected;


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    RaisePropertyChanged(nameof(IsSelected));
                }
            }
        }

        public SheetWrapper(ViewSheet sheet)
        {
            SheetNumber = sheet.SheetNumber;
            SheetName = sheet.Name;
            Id = sheet.Id;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

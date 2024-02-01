using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.ComponentModel;

namespace Loop.Revit.ViewTitles.Helpers
{
    public class SheetWrapper : INotifyPropertyChanged
    {
        public string SheetNumber   { get; set; }
        public string SheetName { get; set; }
        public ElementId Id { get; set; }

        public ICollection<ElementId> ViewportIds { get; set; }

        private bool _isSelected;


        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));

                }
            }
        }

        public SheetWrapper(ViewSheet sheet)
        {
            SheetNumber = sheet.SheetNumber;
            SheetName = sheet.Name;
            Id = sheet.Id;
            ViewportIds = sheet.GetAllViewports();
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Linq;

namespace Loop.Revit.FavouriteViews
{
    public class ViewWrapper : INotifyPropertyChanged
    {
        public string ViewType { get; set; }
        public string ViewName { get; set; }
        public bool IsFavourite { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewWrapper(View view)
        {
            ViewName = view.Name;
            ViewType = view.ViewType.ToString();
        }
    }
}

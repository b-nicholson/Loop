using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.FavouriteViews.Helpers;

namespace Loop.Revit.FavouriteViews
{
    public class ViewWrapper : ObservableObject
    {
        public ElementId ElementId { get; set; }

        public Document Document { get; set; }

        public string ViewType { get; set; }
        public string ViewName { get; set; }
        public bool IsFavourite { get; set; }
        private bool _isDarkMode;
        public bool IsDarkMode
        {
            get => _isDarkMode;
            set
            {
                SetProperty(ref _isDarkMode, value);
                UpdateIcons();
            }
        }
        private BitmapImage _image;
        public BitmapImage Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }
        private ViewIcon _icon;
        public ViewIcon Icon
        {
            get => _icon;
            set => SetProperty(ref _icon, value);
        }
        public ViewWrapper(Document doc, View view, ViewIcon icon)
        {
            Document = doc;
            ElementId = view.Id;
            ViewName = view.Name;
            ViewType = view.ViewType.ToString();
            Icon = icon;
            UpdateIcons();
        }

        private void UpdateIcons()
        {
            Image = IsDarkMode ? Icon.DarkBitmapImage : Icon.LightBitmapImage;
        }
    }
}

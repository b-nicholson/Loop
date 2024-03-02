using System;
using Autodesk.Revit.DB;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.FavouriteViews.Helpers;

namespace Loop.Revit.FavouriteViews
{
    public class ViewWrapper : ObservableObject, IEquatable<ViewWrapper>
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

        public bool Equals(ViewWrapper other)
        {
            if (other == null)
                return false;

            return ElementId.Equals(other.ElementId) && Document.Equals(other.Document);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + ElementId.GetHashCode();
                hash = hash * 23 + Document.GetHashCode();
                return hash;
            }
        }
    }
}

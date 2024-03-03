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
        public string SheetName { get; set; }
        public string SheetNumber { get; set; }
        public string ViewportNumber { get; set; }
        public string DetailReference { get; set; } = "";
        public string DisplaySheetNumber { get; set; }
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
            var viewType = view.ViewType;

            Document = doc;
            ElementId = view.Id;
            ViewName = view.Name;
            SheetName = view.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NAME).AsString();
            SheetNumber = view.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER).AsString();
            ViewportNumber = view.get_Parameter(BuiltInParameter.VIEWPORT_DETAIL_NUMBER).AsString();
            ViewType = viewType.ToString();
            Icon = icon;

            if (ViewportNumber !=  null)
            {
                DetailReference = ViewportNumber + "/" + SheetNumber;
            }

            if (DetailReference != null)
            {
                DisplaySheetNumber = DetailReference;
            }

            if (viewType == Autodesk.Revit.DB.ViewType.DrawingSheet)
            {
                SheetNumber = view.get_Parameter(BuiltInParameter.SHEET_NUMBER).AsString();
                DisplaySheetNumber = SheetNumber;
            }

            
            
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

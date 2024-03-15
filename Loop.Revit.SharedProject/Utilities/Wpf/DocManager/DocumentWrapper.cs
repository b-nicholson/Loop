using Autodesk.Revit.DB;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.FavouriteViews;
using Color = System.Windows.Media.Color;

namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public class DocumentWrapper : ObservableObject
    {
        public Document Doc { get; set; }

        public Color Color { get; set; }

        public List<ViewWrapper> RecentViews { get; set; }

        public string Name { get; set; }

        private bool _showRecentViews;
        public bool ShowRecentViews
        {
            get => _showRecentViews;
            set => SetProperty(ref _showRecentViews, value);
        }

        public DocumentWrapper(Document doc, Color color)
        {
            Doc = doc;
            Color = color;
            Name = doc.Title;
            RecentViews = new List<ViewWrapper>();
        }
    }
}

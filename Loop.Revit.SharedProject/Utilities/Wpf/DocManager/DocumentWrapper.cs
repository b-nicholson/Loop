using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.FavouriteViews;
using Loop.Revit.ViewTitles.Helpers;
using Color = System.Windows.Media.Color;
using System.Windows.Data;
using System;

namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public class DocumentWrapper : ObservableObject
    {
        public Document Doc { get; set; }

        public Color Color { get; set; }

        public List<ViewWrapper> RecentViews { get; set; }

        private ICollectionView _newRecentViews;
        public ICollectionView NewRecentViews {
            get => _newRecentViews; 
            set=> SetProperty(ref _newRecentViews, value);}

        public ObservableCollection<ViewWrapper> ViewCollection= new ObservableCollection<ViewWrapper>();


        private string _query;
        public string Query
        {
            get => _query;
            set
            {
                SetProperty(ref _query, value);
                try
                {
                    NewRecentViews.Filter = FilterViews;
                }
                catch (Exception)
                {
                    // this can throw exceptions for stupid reasons, just ignore
                }
            }
        }
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

            NewRecentViews = CollectionViewSource.GetDefaultView(ViewCollection);
            NewRecentViews.Filter = FilterViews;

        }

        private bool FilterViews(object obj)
        {
            if (Query == null)
            {
                return true;
            }
            var view = (ViewWrapper)obj;
            return view.SheetName != null && view.SheetName.Contains(Query);
            //return true;
        }
    }
}

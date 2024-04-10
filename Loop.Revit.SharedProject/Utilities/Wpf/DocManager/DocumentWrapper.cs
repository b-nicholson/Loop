using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.FavouriteViews;
using Color = System.Windows.Media.Color;
using System.Windows.Data;
using System;

namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public class DocumentWrapper : ObservableObject
    {
        public Document Doc { get; set; }
        public Color Color { get; set; }

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
                NewRecentViews.Filter = FilterViews;
            }
        }
        public string Name { get; set; }

        public bool IsFamilyDocument { get; set; }

        private bool _showRecentViews;
        public bool ShowRecentViews
        {
            get => _showRecentViews;
            set => SetProperty(ref _showRecentViews, value);
        }

        //Filters
        #region ViewWrapperFilterBools
        private bool _areaPlan = true;
        public bool AreaPlan
        {
            get => _areaPlan;
            set
            {
                if (SetProperty(ref _areaPlan, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }
        private bool _ceilingPlan = true;
        public bool CeilingPlan
        {
            get => _ceilingPlan;
            set
            {
                if (SetProperty(ref _ceilingPlan, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _columnSchedule = true;
        public bool ColumnSchedule
        {
            get => _columnSchedule;
            set
            {
                if (SetProperty(ref _columnSchedule, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _detail = true;
        public bool Detail
        {
            get => _detail;
            set
            {
                if (SetProperty(ref _detail, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _draftingView = true;
        public bool DraftingView
        {
            get => _draftingView;
            set
            {
                if (SetProperty(ref _draftingView, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _drawingSheet = true;
        public bool DrawingSheet
        {
            get => _drawingSheet;
            set
            {
                if (SetProperty(ref _drawingSheet, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _elevation = true;
        public bool Elevation
        {
            get => _elevation;
            set
            {
                if (SetProperty(ref _elevation, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _engineeringPlan = true;
        public bool EngineeringPlan
        {
            get => _engineeringPlan;
            set
            {
                if (SetProperty(ref _engineeringPlan, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _floorPlan = true;
        public bool FloorPlan
        {
            get => _floorPlan;
            set
            {
                if (SetProperty(ref _floorPlan, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _legend = true;
        public bool Legend
        {
            get => _legend;
            set
            {
                if (SetProperty(ref _legend, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _panelSchedule = true;
        public bool PanelSchedule
        {
            get => _panelSchedule;
            set
            {
                if (SetProperty(ref _panelSchedule, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _rendering = true;
        public bool Rendering
        {
            get => _rendering;
            set
            {
                if (SetProperty(ref _rendering, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _report = true;
        public bool Report
        {
            get => _report;
            set
            {
                if (SetProperty(ref _report, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _section = true;
        public bool Section
        {
            get => _section;
            set
            {
                if (SetProperty(ref _section, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _schedule = true;
        public bool Schedule
        {
            get => _schedule;
            set
            {
                if (SetProperty(ref _schedule, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _threeD = true;
        public bool ThreeD
        {
            get => _threeD;
            set
            {
                if (SetProperty(ref _threeD, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _walkthrough = true;
        public bool Walkthrough
        {
            get => _walkthrough;
            set
            {
                if (SetProperty(ref _walkthrough, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        // Sheet Params
        private bool _searchSheetNum = true;
        public bool SearchSheetNum
        {
            get => _searchSheetNum;
            set
            {
                if (SetProperty(ref _searchSheetNum, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _searchSheetName = true;
        public bool SearchSheetName
        {
            get => _searchSheetName;
            set
            {
                if (SetProperty(ref _searchSheetName, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }

        private bool _searchViewName = true;
        public bool SearchViewName
        {
            get => _searchViewName;
            set
            {
                if (SetProperty(ref _searchViewName, value))
                {
                    NewRecentViews.Filter = FilterViews;
                }
            }
        }
        #endregion

        public DocumentWrapper(Document doc, Color color)
        {
            Doc = doc;
            Color = color;
            Name = doc.Title.Replace(".rfa", "");
            IsFamilyDocument = doc.IsFamilyDocument;
            NewRecentViews = CollectionViewSource.GetDefaultView(ViewCollection);
        }

        public void UpdatePropertyByName<T>(string propertyName, T newValue)
        {
            var propertyInfo = this.GetType().GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.CanWrite)
            {
                // Get the current value
                T currentValue = (T)propertyInfo.GetValue(this);
                // Use SetProperty to update the value if it has changed, which also raises PropertyChanged
                SetProperty(ref currentValue, newValue, propertyName);
                // Write back the possibly updated value in case SetProperty changed the reference
                propertyInfo.SetValue(this, currentValue, null);
            }
        }

        public void RefreshICollectionFilter()
        {
            NewRecentViews.Filter = FilterViews;
        }

        private bool FilterViews(object obj)
        {
            try
            {
                var view = (ViewWrapper)obj;
                var predicates = new List<Func<ViewWrapper, bool>>
                {
                    v => AreaPlan || v.ViewType != ViewType.AreaPlan,
                    v => CeilingPlan || v.ViewType != ViewType.CeilingPlan,
                    v => ColumnSchedule || v.ViewType != ViewType.ColumnSchedule,
                    v => Detail || v.ViewType != ViewType.Detail,
                    v => DraftingView || v.ViewType != ViewType.DraftingView,
                    v => DrawingSheet || v.ViewType != ViewType.DrawingSheet,
                    v => Elevation || v.ViewType != ViewType.Elevation,
                    v => EngineeringPlan || v.ViewType != ViewType.EngineeringPlan,
                    v => FloorPlan || v.ViewType != ViewType.FloorPlan,
                    v => Legend || v.ViewType != ViewType.Legend,
                    v => PanelSchedule || v.ViewType != ViewType.PanelSchedule,
                    v => Rendering || v.ViewType != ViewType.Rendering,
                    v => Report || v.ViewType != ViewType.Report,
                    v => Section || v.ViewType != ViewType.Section,
                    v => Schedule || v.ViewType != ViewType.Schedule,
                    v => ThreeD || v.ViewType != ViewType.ThreeD,
                    v => Walkthrough || v.ViewType != ViewType.Walkthrough,
                };

                foreach (var predicate in predicates)
                {
                    if (!predicate(view))
                    {
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(Query))
                {
                    var checkList = new List<bool>();
                    if (SearchSheetName)
                    {
                       checkList.Add(!string.IsNullOrEmpty(view.SheetName) && view.SheetName.IndexOf(Query, StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    if (SearchSheetNum)
                    {
                        checkList.Add(!string.IsNullOrEmpty(view.DisplaySheetNumber) && view.DisplaySheetNumber.IndexOf(Query, StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    if (SearchViewName)
                    {
                        checkList.Add(!string.IsNullOrEmpty(view.ViewName) && view.ViewName.IndexOf(Query, StringComparison.OrdinalIgnoreCase) >= 0);
                    }
                    if (!(checkList.Contains(true)))
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                //This can sometimes be stupid, ignore and move on
                return true;
            }
        }
    }
}

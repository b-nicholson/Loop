using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.FavouriteViews.Helpers;
using Loop.Revit.Utilities.Wpf.DocManager;
using Loop.Revit.Utilities.Wpf.SmallDialog;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.FavouriteViews
{
    public class DockablePanelViewModel : ObservableObject
    {
        public DockablePanelModel Model { get; set; }
        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();
        public RelayCommand LoadViews { get; set; }
        public RelayCommand<ViewWrapper> RightClick1 { get; set; }
        public RelayCommand<ViewWrapper> DoubleClick { get; set; }
        public RelayCommand<DocumentWrapper> DocumentRightClickCloseDoc { get; set; }
        public RelayCommand<DocumentWrapper> DocumentRightClickGoToStartupView { get; set; }
        public RelayCommand<DocumentWrapper> DocumentRightClickGoToStartupViewAndClose { get; set; }
        public RelayCommand<DocumentWrapper> DocumentRightCLickClearRecentViews { get; set; }

        private ICollectionView _visibleCollection;
        public ICollectionView VisibleCollection
        {
            get => _visibleCollection; 
            set => SetProperty(ref _visibleCollection, value);
        }

        private ObservableCollection<DocumentWrapper> _documentWrappers = new ObservableCollection<DocumentWrapper>();
        private ObservableCollection<DocumentWrapper> DocumentWrappers
        {
            get => _documentWrappers;
            set => SetProperty(ref _documentWrappers, value);
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
                    UpdateDocFilters(nameof(AreaPlan));
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
                    UpdateDocFilters(nameof(CeilingPlan));
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
                    UpdateDocFilters(nameof(ColumnSchedule));
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
                    UpdateDocFilters(nameof(Detail));
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
                    UpdateDocFilters(nameof(DraftingView));
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
                    UpdateDocFilters(nameof(DrawingSheet));
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
                    UpdateDocFilters(nameof(Elevation));
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
                    UpdateDocFilters(nameof(EngineeringPlan));
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
                    UpdateDocFilters(nameof(FloorPlan));
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
                    UpdateDocFilters(nameof(Legend));
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
                    UpdateDocFilters(nameof(PanelSchedule));
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
                    UpdateDocFilters(nameof(Rendering));
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
                    UpdateDocFilters(nameof(Report));
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
                    UpdateDocFilters(nameof(Section));
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
                    UpdateDocFilters(nameof(Schedule));
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
                    UpdateDocFilters(nameof(ThreeD));
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
                    UpdateDocFilters(nameof(Walkthrough));
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
                    UpdateDocFilters(nameof(SearchSheetNum));
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
                    UpdateDocFilters(nameof(SearchSheetName));
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
                    UpdateDocFilters(nameof(SearchViewName));
                }
            }
        }
        #endregion

        private bool _checkAllViewParams { get; set; }
        public RelayCommand CheckAllViewParams { get; set; }

        public DockablePanelViewModel()
        {
            LoadViews = new RelayCommand(OnLoadViews);

            RightClick1 = new RelayCommand<ViewWrapper>(OnRightClick1);

            DoubleClick = new RelayCommand<ViewWrapper>(OnDoubleClick);

            DocumentRightClickCloseDoc = new RelayCommand<DocumentWrapper>(OnDocumentRightClickCloseDoc);

            DocumentRightClickGoToStartupView = new RelayCommand<DocumentWrapper>(OnDocumentRightClickGoToStartupView);

            DocumentRightClickGoToStartupViewAndClose = 
                new RelayCommand<DocumentWrapper>(OnDocumentRightClickGoToStartupViewAndClose);
            DocumentRightCLickClearRecentViews =
                new RelayCommand<DocumentWrapper>(OnDocumentRightClickClearRecentViews);

            CheckAllViewParams = new RelayCommand(OnCheckAllViewParams);

            VisibleCollection = CollectionViewSource.GetDefaultView(DocumentWrappers);
            
            //VisibleCollection.Filter = FilterViews;

            WeakReferenceMessenger.Default.Register<DockablePanelViewModel, ViewActivatedMessage>(this, (r, m) => r.OnActivatedView(m));
        }

        private void OnCheckAllViewParams()
        {
            _checkAllViewParams = !_checkAllViewParams;
            AreaPlan = _checkAllViewParams;
            CeilingPlan = _checkAllViewParams;
            ColumnSchedule = _checkAllViewParams;
            Detail = _checkAllViewParams;
            DraftingView = _checkAllViewParams;
            DrawingSheet = _checkAllViewParams;
            Elevation = _checkAllViewParams;
            EngineeringPlan = _checkAllViewParams;
            FloorPlan = _checkAllViewParams;
            Legend = _checkAllViewParams;
            PanelSchedule = _checkAllViewParams;
            Rendering = _checkAllViewParams;
            Report = _checkAllViewParams;
            Section = _checkAllViewParams;
            Schedule = _checkAllViewParams;
            ThreeD = _checkAllViewParams;
            Walkthrough = _checkAllViewParams;

        }

        private void UpdateDocFilters(string propertyName)
        {
            foreach (var documentWrapper in DocumentWrappers)
            {
                // Assuming all properties you're dealing with are of type bool for simplicity
                var value = GetType().GetProperty(propertyName)?.GetValue(this, null);
                if (value is bool boolValue)
                {
                    // This calls into the wrapper's method that uses SetProperty internally
                    documentWrapper.UpdatePropertyByName(propertyName, boolValue);
                }
            }

        }

        private void OnDocumentRightClickGoToStartupViewAndClose(DocumentWrapper parameter)
        {
            if (parameter == null) return;
            OnDocumentRightClickGoToStartupView(parameter);
            var doc = parameter.Doc;

            AppCommand.FavouriteViewsHandler.Arg2 = doc;
            AppCommand.FavouriteViewsHandler.Request = RequestId.CloseOpenViews;
            AppCommand.FavouriteViewsEvent.Raise();
        }

        private void OnDocumentRightClickGoToStartupView(DocumentWrapper parameter)
        {
            if (parameter == null) return;
            var doc = parameter.Doc;

            FilteredElementCollector startingViewSettingsCollector =
                new FilteredElementCollector(doc);
            startingViewSettingsCollector.OfClass(typeof(StartingViewSettings));

            Autodesk.Revit.DB.View startingView = null;

            foreach (StartingViewSettings settings in startingViewSettingsCollector)
            {
                startingView = (Autodesk.Revit.DB.View)doc.GetElement(settings.ViewId);
            }

            if (startingView == null)
            {
                MessageQueue.Enqueue(content: "❎ No Startup View Defined");
                return;
            }
            var newWrapper = new ViewWrapper(doc, startingView, IconMapper.GetIcon(startingView));
            AppCommand.FavouriteViewsHandler.Arg1 = newWrapper;
            AppCommand.FavouriteViewsHandler.Request = RequestId.ActivateView;
            AppCommand.FavouriteViewsEvent.Raise();
        }


        private void OnDocumentRightClickClearRecentViews(DocumentWrapper parameter)
        {
            if (parameter != null)
            {
                parameter.ViewCollection.Clear();

            }

        }

        private void OnDocumentRightClickCloseDoc(DocumentWrapper parameter)
        {
            if (parameter == null) return;
            var doc = parameter.Doc;

            if (doc.IsModified)
            {
                var results = SmallDialog.Create(title: "Document Not Saved",
                    message:"There are unsaved changes, do you wish to keep them?",
                    button1:new SdButton("Save", SmallDialogResults.Yes),
                    button2:new SdButton("Discard", SmallDialogResults.No),
                    iconKind:PackIconKind.Warning
                );

                if (results == SmallDialogResults.Yes)
                {
                    if (doc.IsWorkshared)
                    {
                        var swcOpts = new SynchronizeWithCentralOptions();
                        swcOpts.SaveLocalBefore = true;
                        swcOpts.SaveLocalAfter = true;
                        swcOpts.SetRelinquishOptions(new RelinquishOptions(true));
                        doc.SynchronizeWithCentral(new TransactWithCentralOptions(), swcOpts);
                    }
                    else doc.Save();
                }
            }

            try
            {
                doc.Close(false);
            }
            catch (Exception e)
            {
               var docList = ActiveDocumentList.Docs;
               if (docList.Count > 1)
               {
                   foreach (var documentWrapper in docList)
                   {
                       if (Equals(documentWrapper.Doc, doc)) continue;
                       var view = documentWrapper.ViewCollection[0];
                       AppCommand.FavouriteViewsHandler.Arg1 = view;
                       AppCommand.FavouriteViewsHandler.Arg2 = doc;
                       

                 
                        //TODO doesn't work
                       AppCommand.FavouriteViewsHandler.Request = RequestId.SwitchViewAndClose;
                       AppCommand.FavouriteViewsEvent.Raise();


                        AppCommand.FavouriteViewsHandler.Request = RequestId.ActivateView;
                       AppCommand.FavouriteViewsEvent.Raise();



                        break;
                   }
                   return;
               }
               MessageQueue.Enqueue(content: "❎ Can't close the only open document");
            }
        }

        private void OnDoubleClick(ViewWrapper parameter)
        {
            if (parameter != null)
            {
                if (parameter.ElementId != null)
                {
                    AppCommand.FavouriteViewsHandler.Arg1 = parameter;
                    AppCommand.FavouriteViewsHandler.Request = RequestId.ActivateView;
                    AppCommand.FavouriteViewsEvent.Raise();
                }
            }
        }

        private void OnRightClick1(ViewWrapper parameter)
        {
            var hi = 1;
        }
        
        private void OnActivatedView(ViewActivatedMessage message)
        {
            var view = message.NewView;
            
            //Skip the temp view created by the document switching method
            if (view.Name.Contains(FavouriteViewsEventHandler.Prefix))
                return;


            var doc = message.Doc;
            var icon = IconMapper.GetIcon(view);
            var wrapper = new ViewWrapper(doc, view, icon);

            foreach (var docWrapper in ActiveDocumentList.Docs)
            {
                if (Equals(docWrapper.Doc, doc))
                { 
                    docWrapper.ViewCollection.Insert(0, wrapper);
                    //remove old entries
                    docWrapper.ViewCollection = new ObservableCollection<ViewWrapper>(docWrapper.ViewCollection.Distinct().ToList());
                    docWrapper.NewRecentViews = CollectionViewSource.GetDefaultView(docWrapper.ViewCollection);
                    docWrapper.RefreshICollectionFilter();
                }
            }
            DocumentWrappers = new ObservableCollection<DocumentWrapper>(ActiveDocumentList.Docs);
            VisibleCollection = CollectionViewSource.GetDefaultView(DocumentWrappers);
        }

        private void OnClosedView()
        {

        }

        private bool FilterViews(object obj)
        {
            return true;
        }

        private void OnLoadViews()
        {
            //TODO add event handler since we need the event to provide the document context
        }
    }
}

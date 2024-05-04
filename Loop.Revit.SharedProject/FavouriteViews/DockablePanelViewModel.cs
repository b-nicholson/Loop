using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.FavouriteViews.Helpers;
using Loop.Revit.Utilities.UserSettings;
using Loop.Revit.Utilities.Wpf.DocManager;
using Loop.Revit.Utilities.Wpf.SmallDialog;
using MaterialDesignThemes.Wpf;
using Utilities.Wpf.Services.PageServices;

namespace Loop.Revit.FavouriteViews
{
    public class DockablePanelViewModel : ObservableObject
    {
        private readonly IPageService _pageService;
        public DockablePanelModel Model { get; set; }
        public SnackbarMessageQueue MessageQueue { get; } = new SnackbarMessageQueue();
        public RelayCommand LoadViews { get; set; }
        public RelayCommand RefreshViews { get; set; }

        public RelayCommand<DocumentWrapper> MoveDocumentUp { get; set; }
        public RelayCommand<DocumentWrapper> MoveDocumentDown { get; set; }
        public RelayCommand<Tuple<object, object>> RightClickRemove { get; set; }
        public RelayCommand<Tuple<object, object>> ViewRightClickAddToFavourites { get; set; }
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

        private bool _checkAllViewParams { get; set; } = true;
        public RelayCommand CheckAllViewParams { get; set; }

        public DockablePanelViewModel(IPageService pageService)
        {
            _pageService = pageService;

            MoveDocumentUp = new RelayCommand<DocumentWrapper>(OnMoveDocumentUp);
            MoveDocumentDown = new RelayCommand<DocumentWrapper>(OnMoveDocumentDown);

            LoadViews = new RelayCommand(OnLoadViews);

            RefreshViews = new RelayCommand(OnRefreshViews);

            RightClickRemove = new RelayCommand<Tuple<object, object>>(OnRightClickRemove);

            ViewRightClickAddToFavourites = new RelayCommand<Tuple<object, object>>(OnViewRightClickAddToFavourites);

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
            WeakReferenceMessenger.Default.Register<DockablePanelViewModel, DocumentSwitchMessage>(this, (r, m) => r.OnDelayedCloseDoc(m));
            WeakReferenceMessenger.Default.Register<DockablePanelViewModel, ColourChangedMessage>(this, (r, m) => r.OnColourChanged());
        }

        public void OnMoveDocumentUp(DocumentWrapper docWrapper)
        {
            var docIndex = ActiveDocumentList.Docs.IndexOf(docWrapper);
            if (docIndex > 0)
            {
                var itemToMove = ActiveDocumentList.Docs[docIndex];
                ActiveDocumentList.Docs.RemoveAt(docIndex);
                ActiveDocumentList.Docs.Insert(docIndex - 1, itemToMove);
            }
            RefreshDocumentList();
        }

        public void OnMoveDocumentDown(DocumentWrapper docWrapper)
        {
            var docIndex = ActiveDocumentList.Docs.IndexOf(docWrapper);
            var listLength = ActiveDocumentList.Docs.Count - 1;

            if (docIndex != listLength)
            {
                var itemToMove = ActiveDocumentList.Docs[docIndex];
                ActiveDocumentList.Docs.RemoveAt(docIndex);
                ActiveDocumentList.Docs.Insert(docIndex + 1, itemToMove);
            }
            RefreshDocumentList();
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

        private void OnDelayedCloseDoc(DocumentSwitchMessage message)
        {
            if (message.RequestId == RequestId.CloseDocument)
            {
                var doc = message.Document;
                AppCommand.FavouriteViewsHandler.Request = RequestId.CloseDocument;
                AppCommand.FavouriteViewsHandler.Arg2 = doc;
                AppCommand.FavouriteViewsEvent.Raise();
            }

            if (message.RequestId == RequestId.RefreshViews)
            {
                RefreshDocumentList();
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
                RefreshDocumentList();
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
                       
                       AppCommand.FavouriteViewsHandler.Request = RequestId.SwitchViewAndQueueClose;
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

        private void OnRightClickRemove(Tuple<object, object> parameter)
        {
            var viewWrapper =(ViewWrapper)parameter.Item1;
            var docWrapper = (DocumentWrapper)parameter.Item2;
            docWrapper.ViewCollection.Remove(viewWrapper);
        }

        private void OnViewRightClickAddToFavourites(Tuple<object, object> parameter)
        {
            var viewWrapper = (ViewWrapper)parameter.Item1;
            var docWrapper = (DocumentWrapper)parameter.Item2;
            docWrapper.ViewCollection.Remove(viewWrapper);

        }


        private void OnActivatedView(ViewActivatedMessage message)
        {
            var view = message.NewView;

            //Skip the temp view created by the document switching method
            if (view.Name.Contains(FavouriteViewsEventHandler.Prefix))
                return;

            // Editing a family inside the document, rather than file>open doesn't trigger a document opened event
            // Need to add the document here rather than use a different event handler. Expensive, so better to only test families
            if (message.Doc.IsFamilyDocument)
            {
                bool docListContains = ActiveDocumentList.Docs.Any(docWrapper => Equals(docWrapper.Doc, message.Doc));
                if (!docListContains)
                {
                    DocumentWrapperHelper.AddNewDocument(message.Doc);
                }
            }

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
            RefreshDocumentList();
        }

        private void RefreshDocumentList()
        {
            DocumentWrappers = new ObservableCollection<DocumentWrapper>(ActiveDocumentList.Docs);
            VisibleCollection = CollectionViewSource.GetDefaultView(DocumentWrappers);
        }

        private void OnClosedView()
        {

        }

        private void OnRefreshViews()
        {
            var docWrapperList = ActiveDocumentList.Docs;

            foreach (var docWrapper in docWrapperList)
            {
                var viewWrapperList = docWrapper.ViewCollection;
                var newViewWrapperList = new ObservableCollection<ViewWrapper>();
                foreach (var viewWrapper in viewWrapperList)
                {
                    var viewId = viewWrapper.ElementId;
                    var doc = viewWrapper.Document;

                    var updatedView = doc.GetElement(viewId);
                    if (updatedView == null) continue;

                    try
                    {
                        var newView = (View)updatedView;
                        var icon = IconMapper.GetIcon(newView);
                        var newWrapper = new ViewWrapper(doc, newView, icon);
                        newViewWrapperList.Add(newWrapper);
                    }
                    catch (Exception)
                    {
                        //pass
                    }
                }
                docWrapper.ViewCollection = newViewWrapperList;
                docWrapper.NewRecentViews = CollectionViewSource.GetDefaultView(docWrapper.ViewCollection);
                docWrapper.RefreshICollectionFilter();
                OnColourChanged();
            }
        }
        private void LoadSettings()
        {
        }

        private void OnColourChanged()
        {
            //#if !(Revit2022 || Revit2023)
            //var darkmode = GlobalSettings.Settings.IsDarkModeTheme;
            //_pageService.ToggleDarkMode(darkmode);

            //foreach (var docWrapper in ActiveDocumentList.Docs)
            //{
            //    foreach (var viewWrapper in docWrapper.ViewCollection)
            //    {
            //        viewWrapper.IsDarkMode = darkmode;
            //    }
            //}
            //#endif

            //var colour = GlobalSettings.Settings.PrimaryThemeColor;
            //var theme = _pageService.GetMaterialDesignTheme();
            //theme.SetPrimaryColor(colour);
            //_pageService.SetMaterialDesignTheme(theme);
            AppCommand.FavouriteViewsHandler.Arg1 = _pageService;
            AppCommand.FavouriteViewsHandler.Request = RequestId.RefreshTheme;
            AppCommand.FavouriteViewsEvent.Raise();

        }

        private void OnLoadViews()
        {
            LoadSettings();
            //TODO add event handler since we need the event to provide the document context
        }
    }
}

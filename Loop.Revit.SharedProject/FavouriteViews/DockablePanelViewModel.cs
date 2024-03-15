﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.FavouriteViews.Helpers;
using Loop.Revit.Utilities.Wpf.DocManager;

namespace Loop.Revit.FavouriteViews
{
    public class DockablePanelViewModel : ObservableObject
    {
        public DockablePanelModel Model { get; set; }
        public RelayCommand LoadViews { get; set; }
        public RelayCommand<object> RowDoubleClickCommand { get; set; }

        private ICollectionView _visibleCollection;
        public ICollectionView VisibleCollection
        {
            get => _visibleCollection; 
            set => SetProperty(ref _visibleCollection, value);
        }


        private ObservableCollection<ViewWrapper> _masterViews = new ObservableCollection<ViewWrapper>();
        private ObservableCollection<ViewWrapper> _simplifiedViews = new ObservableCollection<ViewWrapper>();
        private ObservableCollection<DocumentWrapper> _documentWrappers = new ObservableCollection<DocumentWrapper>();

        private object _selectedTreeItem;
        public object SelectedTreeItem
        {
            get => _selectedTreeItem;
            set => SetProperty(ref _selectedTreeItem, value);
        }

        public RelayCommand<object> TreeItemCommand { get; set; }

        public ICommand SelectionChangedCommand { get; }

        public bool DoNotShowDuplicateViews { get; set; }
        private HashSet<ViewWrapper> seenUniqueViews;

        public DockablePanelViewModel()
        {
            LoadViews = new RelayCommand(OnLoadViews);
            RowDoubleClickCommand = new RelayCommand<object>(OnRowDoubleClick);

            SelectionChangedCommand = new RelayCommand<object>(OnTreeItemClick);
            
            VisibleCollection = CollectionViewSource.GetDefaultView(_documentWrappers);
            
            //VisibleCollection.Filter = FilterViews;
            seenUniqueViews = new HashSet<ViewWrapper>();

            WeakReferenceMessenger.Default.Register<DockablePanelViewModel, ViewActivatedMessage>(this, (r, m) => r.OnActivatedView(m));
        }

        private void OnTreeItemClick(object para)
        {
            var wut = para;

        }

        private void OnRowDoubleClick(object parameter)
        {
            var wtf = SelectedTreeItem;

            var clickedItem = (ViewWrapper)parameter;
            if (clickedItem != null)
            {
                if (clickedItem.ElementId != null)
                {
                    AppCommand.FavouriteViewsHandler.Arg1 = parameter;
                    AppCommand.FavouriteViewsHandler.Request = RequestId.ActivateView;
                    AppCommand.FavouriteViewsEvent.Raise();

                }
            }

        
        }

        private void OnActivatedView(ViewActivatedMessage message)
        {
            var view = message.NewView;
            
            //Skip the temp view created by the document switching method
            if (view.Name.Contains(FavouriteViewsEventHandler.Prefix))
            {
                return;
            }


            var doc = message.Doc;
            var icon = IconMapper.GetIcon(view);
            var wrapper = new ViewWrapper(doc, view, icon);

            foreach (var docWrapper in ActiveDocumentList.Docs)
            {
                if (Equals(docWrapper.Doc, doc))
                {
                    docWrapper.RecentViews.Insert(0, wrapper);
                }
                
            }

          
            _documentWrappers = new ObservableCollection<DocumentWrapper>(ActiveDocumentList.Docs);
            VisibleCollection = CollectionViewSource.GetDefaultView(_documentWrappers);

            _masterViews.Insert(0, wrapper);
            _simplifiedViews.Insert(0, wrapper);

            var viewIsUnique = seenUniqueViews.Add(wrapper);

           
            if (!viewIsUnique && DoNotShowDuplicateViews)
            {
                //View is not unique, remove old instances
                var newList = _simplifiedViews.Distinct().ToList();
                _simplifiedViews = new ObservableCollection<ViewWrapper>(newList);
                VisibleCollection = CollectionViewSource.GetDefaultView(_documentWrappers);
            }
            
        }

        private bool FilterViews(object obj)
        {

            if (DoNotShowDuplicateViews)
            {
                var wrapper = (ViewWrapper)obj;

                var res = seenUniqueViews.Contains(wrapper);
                //var res = wrapper != null && seenUniqueViews.Contains(wrapper);
                return !res;
            }
            else
            {
                return true;
            }
           
        }

        private void OnLoadViews()
        {
            //seenUniqueViews.Clear();
            //_masterViews.Clear();
            //_simplifiedViews.Clear();

            VisibleCollection = CollectionViewSource.GetDefaultView(_masterViews);

            //TODO add event handler since we need the event to provide the document context
        }
    }
}

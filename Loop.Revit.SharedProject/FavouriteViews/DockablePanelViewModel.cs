using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.FavouriteViews.Helpers;

namespace Loop.Revit.FavouriteViews
{
    public class DockablePanelViewModel : ObservableObject
    {
        public DockablePanelModel Model { get; set; }
        public RelayCommand LoadViews { get; set; }

        public RelayCommand<ViewWrapper> RowDoubleClickCommand { get; set; }

        private ICollectionView _visibleCollection;
        public ICollectionView VisibleCollection
        {
            get => _visibleCollection; 
            set => SetProperty(ref _visibleCollection, value);
        }


        private ObservableCollection<ViewWrapper> _masterViews = new ObservableCollection<ViewWrapper>();
        private ObservableCollection<ViewWrapper> _simplifiedViews = new ObservableCollection<ViewWrapper>();

        public bool DoNotShowDuplicateViews { get; set; }
        private HashSet<ViewWrapper> seenUniqueViews;

        public DockablePanelViewModel()
        {
            LoadViews = new RelayCommand(OnLoadViews);
            RowDoubleClickCommand = new RelayCommand<ViewWrapper>(OnRowDoubleClick);
            
            VisibleCollection = CollectionViewSource.GetDefaultView(_simplifiedViews);
            
            //VisibleCollection.Filter = FilterViews;
            seenUniqueViews = new HashSet<ViewWrapper>();

            WeakReferenceMessenger.Default.Register<DockablePanelViewModel, ViewActivatedMessage>(this, (r, m) => r.OnActivatedView(m));
        }

        private void OnRowDoubleClick(ViewWrapper parameter)
        {
            AppCommand.FavouriteViewsHandler.Arg1 = parameter;
            AppCommand.FavouriteViewsHandler.Request = RequestId.ActivateView;
            AppCommand.FavouriteViewsEvent.Raise();
        }

        private void OnActivatedView(ViewActivatedMessage message)
        {
            var view = message.NewView;
            var doc = message.Doc;
            var icon = IconMapper.GetIcon(view);
            var wrapper = new ViewWrapper(doc, view, icon);

            _masterViews.Insert(0, wrapper);
            _simplifiedViews.Insert(0, wrapper);

            var viewIsUnique = seenUniqueViews.Add(wrapper);

           
            if (!viewIsUnique)
            {
                //View is not unique, remove old instances
                var newList = _simplifiedViews.Distinct().ToList();
                _simplifiedViews = new ObservableCollection<ViewWrapper>(newList);
                VisibleCollection.Refresh();

            }
     

            

            
            //_masterViews.Add(wrapper);
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
            seenUniqueViews.Clear();
            _masterViews.Clear();

            //TODO add event handler since we need the event to provide the document context
        }
    }
}

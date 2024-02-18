using System.Collections.ObjectModel;
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

        private ObservableCollection<ViewWrapper> _views = new ObservableCollection<ViewWrapper>();
        public ObservableCollection<ViewWrapper> Views
        {
            get { return _views; }
            set
            {
                _views = value; 
                OnPropertyChanged(nameof(Views));
            }
        }

        public DockablePanelViewModel()
        {
            LoadViews = new RelayCommand(OnLoadViews);
            RowDoubleClickCommand = new RelayCommand<ViewWrapper>(OnRowDoubleClick);


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
            Views.Add(wrapper);

        }

        private void OnLoadViews()
        {
            //Views = Model.CollectViews();

            //TODO add event handler since we need the event to provide the document context
        }
    }
}

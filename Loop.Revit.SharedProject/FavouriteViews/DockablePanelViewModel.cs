using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Loop.Revit.FavouriteViews
{
    public class DockablePanelViewModel : ViewModelBase
    {

        public DockablePanelModel Model { get; set; }

        public RelayCommand LoadViews { get; set; }

        private ObservableCollection<ViewWrapper> _views = new ObservableCollection<ViewWrapper>();
        public ObservableCollection<ViewWrapper> Views
        {
            get { return _views; }
            set { _views = value; RaisePropertyChanged(() => Views);}
        }

        public DockablePanelViewModel()
        {
            LoadViews = new RelayCommand(OnLoadViews);
        }

        private void OnLoadViews()
        {
            //Views = Model.CollectViews();

            //TODO add event handler since we need the event to provide the document context
        }
    }
}

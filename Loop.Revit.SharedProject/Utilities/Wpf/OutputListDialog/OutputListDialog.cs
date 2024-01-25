using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Data;
using Loop.Revit.Utilities.Wpf.WindowServices;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public class OutputListDialog
    {
        public static void Create(IEnumerable<object> data, bool modeless = false, Window owner = null, ITheme theme = null)
        {
            var view = new OutputListDialogView();

            var viewModel = new OutputListDialogViewModel(
                windowService: new WindowService(view), 
                theme: theme
            );
            var observableData = new ObservableCollection<object>(data);
            var dataView = CollectionViewSource.GetDefaultView(observableData);


            viewModel.DataGridElements = dataView;
            view.DataContext = viewModel;
            view.ShowInTaskbar = true;
            //if (owner != null)
            //{
            //    view.Owner = owner;
            //}
            //TODO check if modal/modeless is useful. Not sure if usable in modeless.
            if (modeless) view.Show();
            //else view.ShowDialog();
        }
    }
}

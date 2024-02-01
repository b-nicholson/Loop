using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Loop.Revit.Utilities.Wpf.WindowServices;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public class OutputListDialog
    {
        public static void Create(IEnumerable<object> data, ObservableCollection<DataGridColumnModel> columns, string title, UIDocument uiDoc = null, bool modeless = false, Window owner = null, ITheme theme = null)
        {
            OnCreate(data: data, columns: columns, title: title, uiDoc: uiDoc, modeless: modeless, owner: owner, theme: theme);
        }
        public static void Create(OutputDialogListMessage message)
        {
            //same thing, the args are just wrapped in a message class
            var data = message.Data;
            var columns = message.Columns;
            var title = message.Title;
            var uiDoc = message.UiDoc;
            var modeless = message.Modeless;
            var theme = message.Theme;
            var owner = message.Owner;
            
            OnCreate(data: data, columns:columns,title:title,uiDoc:uiDoc, modeless:modeless, owner:owner, theme:theme);
        }

        private static void OnCreate(IEnumerable<object> data, ObservableCollection<DataGridColumnModel> columns, string title, UIDocument uiDoc = null, bool modeless = false, Window owner = null, ITheme theme = null)
        {
            //single method to execute from same overloaded input
            var view = new OutputListDialogView();

            var viewModel = new OutputListDialogViewModel(
                title: title,
                windowService: new WindowService(view),
                columns: columns,
                theme: theme
            );
            var observableData = new ObservableCollection<object>(data);
            var dataView = CollectionViewSource.GetDefaultView(observableData);

            if (uiDoc != null)
            {
                viewModel.uiDoc = uiDoc;
            }

            viewModel.DataGridElements = dataView;
            view.DataContext = viewModel;
            view.ShowInTaskbar = true;

            view.Owner = null;
            if (owner != null)
            {
                view.Owner = owner;
            }
            //TODO check if modal/modeless is useful. Not sure if usable in modeless.
            if (modeless) view.Show();
            else view.ShowDialog();
        }
    }
}

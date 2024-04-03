using System;
using Autodesk.Revit.UI;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using MenuItem = System.Windows.Controls.MenuItem;


namespace Loop.Revit.FavouriteViews
{
    public sealed partial class DockablePanelPage : IDockablePaneProvider
    {
        public DockablePanelPage()
        {
            // Useless dummy code to force revit to load the libraries https://stackoverflow.com/questions/55594443/how-to-include-materialdesignxamltoolkit-to-wpf-class-library
            ColorZoneAssist.SetMode(new System.Windows.Controls.GroupBox(), ColorZoneMode.Light);
            new Hue("name", System.Windows.Media.Color.FromArgb(1, 2, 3, 4), System.Windows.Media.Color.FromArgb(1, 5, 6, 7));

            // Dummy code to force the loading of Microsoft.Xaml.Behaviors.Wpf assembly
            var dummyType = typeof(Behavior);

            InitializeComponent();
        }

        private void OnDocumentRightClickClearRecent(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var selectedItem = menuItem.CommandParameter;

            var viewModel = DataContext as DockablePanelViewModel;
            if (viewModel != null)
            {
                viewModel?.DocumentRightCLickClearRecentViews.Execute(selectedItem);
            }
        }

        private void OnDocumentRightClickGoToStartupViewAndClose(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var selectedItem = menuItem.CommandParameter;

            var viewModel = DataContext as DockablePanelViewModel;
            if (viewModel != null)
            {
                viewModel?.DocumentRightClickGoToStartupViewAndClose.Execute(selectedItem);
            }
        }

        private void OnDocumentRightClickGoToStartupView(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var selectedItem = menuItem.CommandParameter;

            var viewModel = DataContext as DockablePanelViewModel;
            if (viewModel != null)
            {
                viewModel?.DocumentRightClickGoToStartupView.Execute(selectedItem);
            }
        }

        private void OnDocumentRightClickCloseDoc(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;
            var selectedItem = menuItem.CommandParameter;

            var viewModel = DataContext as DockablePanelViewModel;
            if (viewModel != null)
            {
                viewModel?.DocumentRightClickCloseDoc.Execute(selectedItem);
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid?.SelectedItem == null) return;

            var selectedItem = dataGrid.SelectedItem;

            var viewModel = DataContext as DockablePanelViewModel;
            if (viewModel != null)
            {
                viewModel?.DoubleClick.Execute(selectedItem);
            }
        }

        private void OnRightClickRemove(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            if (menuItem == null) return;

            var selectedItem = menuItem.CommandParameter;
            if (selectedItem == null) return;
            var context = menuItem.DataContext;

            var param = new Tuple<object, object>(selectedItem, context);

            var viewModel = DataContext as DockablePanelViewModel;
            viewModel?.RightClickRemove.Execute(param);
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            data.InitialState = new DockablePaneState()
            {
                DockPosition = DockPosition.Tabbed,
                TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
            };
            data.VisibleByDefault = true;
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace Loop.Revit.Utilities.Wpf.TreeViewUtils
{
    public static class TreeViewExtensions
    {
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached("SelectedItem", typeof(object), typeof(TreeViewExtensions), new PropertyMetadata(null, OnSelectedItemChanged));

        public static object GetSelectedItem(DependencyObject target) => target.GetValue(SelectedItemProperty);

        public static void SetSelectedItem(DependencyObject target, object value) => target.SetValue(SelectedItemProperty, value);

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var treeView = sender as TreeView;
            if (treeView != null)
            {
                treeView.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
                treeView.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            }
        }

        private static void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;
            if (tree != null)
            {
                SetSelectedItem(tree, tree.SelectedItem);
            }
        }
    }
}

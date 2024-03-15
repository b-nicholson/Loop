using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public static class TreeViewItemBehavior
    {
        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.RegisterAttached(
                "DoubleClickCommand",
                typeof(ICommand),
                typeof(TreeViewItemBehavior),
                new UIPropertyMetadata(null, OnDoubleClickCommandChanged));

        public static void SetDoubleClickCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(DoubleClickCommandProperty, value);
        }

        public static ICommand GetDoubleClickCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(DoubleClickCommandProperty);
        }

        private static void OnDoubleClickCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is TreeViewItem item)
            {
                if (e.NewValue != null)
                {
                    item.MouseDoubleClick += Item_MouseDoubleClick;
                }
                else
                {
                    item.MouseDoubleClick -= Item_MouseDoubleClick;
                }
            }
        }

        private static void Item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = sender as TreeViewItem;
            if (treeViewItem != null)
            {
                ICommand command = GetDoubleClickCommand(treeViewItem);
                if (command != null && command.CanExecute(treeViewItem.DataContext))
                {
                    command.Execute(treeViewItem.DataContext);
                }
            }
        }
    }
}

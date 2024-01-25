using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Loop.Revit.Utilities.Wpf.DataGridUtils
{
    public static class DataGridColumnsBinding
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached("BindableColumns", typeof(ObservableCollection<DataGridColumnModel>), typeof(DataGridColumnsBinding), new UIPropertyMetadata(null, BindableColumnsPropertyChanged));

        private static void BindableColumnsPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            DataGrid dataGrid = source as DataGrid;
            ObservableCollection<DataGridColumnModel> columns = e.NewValue as ObservableCollection<DataGridColumnModel>;

            dataGrid.Columns.Clear();
            if (columns == null)
            {
                return;
            }

            foreach (DataGridColumnModel columnModel in columns)
            {
                DataGridTextColumn column = new DataGridTextColumn
                {
                    Header = columnModel.Header,
                    Binding = new Binding(columnModel.BindingPath),
                    Width = columnModel.Width
                };
                dataGrid.Columns.Add(column);
            }
        }

        public static void SetBindableColumns(DependencyObject element, ObservableCollection<DataGridColumnModel> value)
        {
            element.SetValue(BindableColumnsProperty, value);
        }

        public static ObservableCollection<DataGridColumnModel> GetBindableColumns(DependencyObject element)
        {
            return (ObservableCollection<DataGridColumnModel>)element.GetValue(BindableColumnsProperty);
        }
    }
}

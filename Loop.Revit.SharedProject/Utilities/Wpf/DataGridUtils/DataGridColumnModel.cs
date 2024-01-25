using System.Windows.Controls;

namespace Loop.Revit.Utilities.Wpf
{
    public class DataGridColumnModel
    {
        public string Header { get; set; }
        public string BindingPath { get; set; }
        public DataGridLength Width { get; set; }
        // You can add more properties as needed, such as sort direction, visibility, etc.
    }
}

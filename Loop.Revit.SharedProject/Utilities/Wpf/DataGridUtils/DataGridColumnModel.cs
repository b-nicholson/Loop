using System.Windows.Controls;
using System.Windows.Input;

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

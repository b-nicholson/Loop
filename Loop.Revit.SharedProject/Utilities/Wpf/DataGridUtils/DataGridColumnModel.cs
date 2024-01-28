using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Loop.Revit.Utilities.Wpf
{
    public class DataGridColumnModel
    {
        public string Header { get; set; }
        public string BindingPath { get; set; }
        public DataGridLength Width { get; set; }

        public TextAlignment HeaderTextAlignment { get; set; } = TextAlignment.Left;

        public HorizontalAlignment HeaderContentAlignment { get; set; } = HorizontalAlignment.Left;

        public BindingMode BindingMode { get; set; } = BindingMode.TwoWay;
        // You can add more properties as needed, such as sort direction, visibility, etc.
    }
}

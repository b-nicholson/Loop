using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.DataGridUtils
{
    public class DataGridButtonColumnModel : DataGridColumnModel
    {
        public ICommand Command { get; set; }
        public string CommandParameterPath { get; set; }

        public bool ContentIsBinding { get; set; }

        private object _content { get; set; }

        public object Content
        {
            get => _content;
            set
            {
                _content = value;
                if (!ContentIsBinding)
                {
                    ContentIsPackIcon = value.GetType() == typeof(PackIconKind);
                }
            }
        }

        public bool ContentIsPackIcon { get; set; }

        // Add other properties related to button appearance if needed
    }
}

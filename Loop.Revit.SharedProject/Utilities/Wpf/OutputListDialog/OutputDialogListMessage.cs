using Autodesk.Revit.UI;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using System.Windows;


namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public class OutputDialogListMessage
    {
        public IEnumerable<object> Data{ get; set; }

        public ObservableCollection<DataGridColumnModel> Columns { get; set; }

        public string Title { get; set; }

        public UIDocument UiDoc { get; set; }

        public bool Modeless { get; set; }

        public Window Owner { get; set; }

        public ITheme Theme { get; set; }


        public OutputDialogListMessage(IEnumerable<object> data, ObservableCollection<DataGridColumnModel> columns, string title, UIDocument uiDoc = null, bool modeless = false, Window owner = null, ITheme theme = null)
        {
            Data = data;
            Columns = columns; Title = title;
            UiDoc = uiDoc;
            Theme = theme;
            Modeless = modeless;
            Owner = owner;
        }
    }
}

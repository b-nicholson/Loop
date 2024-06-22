using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Loop.Revit.ShapeEdits
{
    public class ShapeEditsModel
    {
        public UIApplication UiApp { get; }
        public UIDocument UiDoc { get; }
        public Document Doc { get; }
        public View ActiveView { get; set; }

        public ShapeEditsModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            UiDoc = UiApp.ActiveUIDocument;
            Doc = UiDoc.Document;
            ActiveView = Doc.ActiveView;
        }
    }
}

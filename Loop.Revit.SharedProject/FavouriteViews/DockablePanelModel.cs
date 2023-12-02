using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.Linq;


namespace Loop.Revit.FavouriteViews
{
    public class DockablePanelModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }
        public DockablePanelModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = uiApp.ActiveUIDocument.Document;
        }

        public ObservableCollection<ViewWrapper> CollectViews()
        {
            var spatialObjects = new FilteredElementCollector(Doc).OfClass(typeof(View))
                .WhereElementIsNotElementType().Cast<View>()
                .Select(x => new ViewWrapper(x));

            return new ObservableCollection<ViewWrapper>(spatialObjects);
        }


    }
}

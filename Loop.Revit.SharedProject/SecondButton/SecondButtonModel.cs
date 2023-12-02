using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Loop.Revit.SecondButton
{
    public class SecondButtonModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }
        public SecondButtonModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = uiApp.ActiveUIDocument.Document;
        }

        public ObservableCollection<SpatialObjectWrapper> CollectSpatialObjects()
        {
            var spatialObjects = new FilteredElementCollector(Doc).OfClass(typeof(SpatialElement))
                .WhereElementIsNotElementType().Cast<SpatialElement>().Where(x => x is Room)
                .Select(x => new SpatialObjectWrapper(x));

            return new ObservableCollection<SpatialObjectWrapper>(spatialObjects);
        }

        public void Delete(List<SpatialObjectWrapper> selected)
        {
            var ids = selected.Select(x => x.Id).ToList();
            using (var trans = new Transaction(Doc, "Delete Rooms"))
            {
                trans.Start();
                Doc.Delete(ids);
                trans.Commit();
            }
        }

    }
}

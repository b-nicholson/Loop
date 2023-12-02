using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace Loop.Revit.ThirdButton
{
    public class ThirdButtonModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }
        public ThirdButtonModel(UIApplication uiApp)
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
            AppCommand.ThirdButtonHandler.Arg1 = selected;
            AppCommand.ThirdButtonHandler.Request = RequestId.Delete;
            AppCommand.ThirdButtonEvent.Raise();
        }

    }
}

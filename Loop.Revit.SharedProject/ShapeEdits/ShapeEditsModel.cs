using System.Collections.Generic;
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

        public void Project(List<Element> targets, List<Element> hostElements, bool ignoreInternalPoints, bool boundaryPointOnly, double verticalOffset)
        {
            AppCommand.ShapeEditsHandler.Targets = targets;
            AppCommand.ShapeEditsHandler.HostElements = hostElements;
            AppCommand.ShapeEditsHandler.IgnoreInternalPoints = ignoreInternalPoints;
            AppCommand.ShapeEditsHandler.BoundaryPointOnly = boundaryPointOnly;
            AppCommand.ShapeEditsHandler.VerticalOffset = verticalOffset;
            AppCommand.ShapeEditsHandler.Request = RequestId.Project;
            AppCommand.ShapeEditsEvent.Raise();
        }

        public void SelectObjects(bool isTargetElement)
        {
            AppCommand.ShapeEditsHandler.IsTargetElement = isTargetElement;
            AppCommand.ShapeEditsHandler.Request = RequestId.Select;
            AppCommand.ShapeEditsEvent.Raise();
        }
    }
}

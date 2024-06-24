using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Loop.Revit.ShapeEdits.Helpers
{
    public class ShapeEditsSelectionMessage
    {
        public List<Element> SelectedElements { get; set; }
        public bool IsTargetElement { get; set; }

        public ShapeEditsSelectionMessage(List<Element> selectedElements, bool isTargetElement)
        {
            SelectedElements = selectedElements;
            IsTargetElement = isTargetElement;
        }
    }
}

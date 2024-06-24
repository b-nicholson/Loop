using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace Loop.Revit.Utilities.ShapeEdits
{
    public class BoundaryFaceIntersections
    {
        public List<Curve> LineIntersections;
        public List<List<XYZ>> IntersectingPoints;
        public BoundaryFaceIntersections(List<Curve>  lineIntersections, List<List<XYZ>> intersectingPoints)
        {
            IntersectingPoints = intersectingPoints;
            LineIntersections = lineIntersections;
        }

    }
}

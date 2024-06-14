using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Loop.Revit.Utilities.ShapeEdits
{
    public class CurveComparer : IEqualityComparer<Curve>
    {
        public bool Equals(Curve x, Curve y)
        {
            if (x == null || y == null)
                return false;

            XYZ xStart = x.GetEndPoint(0);
            XYZ xEnd = x.GetEndPoint(1);
            XYZ yStart = y.GetEndPoint(0);
            XYZ yEnd = y.GetEndPoint(1);

            // Compare curves regardless of direction
            return (xStart.IsAlmostEqualTo(yStart) && xEnd.IsAlmostEqualTo(yEnd)) ||
                   (xStart.IsAlmostEqualTo(yEnd) && xEnd.IsAlmostEqualTo(yStart));
        }

        public int GetHashCode(Curve obj)
        {
            if (obj == null)
                return 0;

            XYZ start = obj.GetEndPoint(0);
            XYZ end = obj.GetEndPoint(1);

            // Create a hash code that doesn't depend on the direction
            int hash1 = start.GetHashCode() ^ end.GetHashCode();
            int hash2 = end.GetHashCode() ^ start.GetHashCode();

            return hash1 ^ hash2;
        }
    }

}

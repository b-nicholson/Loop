using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.Utilities.ShapeEdits
{
    public class PointProjectionResult
    {
        public List<XYZ> SuccessfulProjections { get; set; }
        public List<XYZ> FailedProjections { get; set; }
        public PointProjectionResult(List<XYZ> successfulProjections, List<XYZ> failedProjections)
        {
            SuccessfulProjections = successfulProjections;
            FailedProjections = failedProjections;
        }
    }
}

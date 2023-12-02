using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.ThirdButton
{
    public class SpatialObjectDeletedMessage
    {
        public List<ElementId> Ids { get; set; }

        public SpatialObjectDeletedMessage(List<ElementId> ids)
        {
            Ids = ids;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.ViewTitles.Helpers
{
    public class NonEditableViewportsMessage
    {
        public List<ViewportWrapper> Viewports { get; set; }

        public NonEditableViewportsMessage(List<ViewportWrapper> viewports)
        {
            Viewports = viewports;
        }
    }
}

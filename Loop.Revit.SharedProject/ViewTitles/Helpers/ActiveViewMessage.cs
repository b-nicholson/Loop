using System;
using System.Collections.Generic;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.ViewTitles.Helpers
{
    public class ActiveViewMessage
    {
        public View ActiveView { get; set; }

        public ActiveViewMessage(View activeView)
        {
            ActiveView = activeView;
        }
    }
}

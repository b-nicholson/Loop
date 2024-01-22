using System;
using System.Collections.Generic;
using System.Security.RightsManagement;
using System.Text;

namespace Loop.Revit.ViewTitles
{
    public class ProgressResultsMessage
    {
        public int TotalSheetCount { get; set; }
        public int CurrentSheetProgress { get; set; }

        public ProgressResultsMessage(int currentSheetProgress, int totalSheetCount)
        {
            TotalSheetCount = totalSheetCount;
            CurrentSheetProgress = currentSheetProgress;
        }
    }
}

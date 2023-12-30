using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{
    public class SmallDialogMessage
    {
        public bool Result { get; set; }

        public SmallDialogMessage(bool result)
        {
            Result = result;
        }
    }
}

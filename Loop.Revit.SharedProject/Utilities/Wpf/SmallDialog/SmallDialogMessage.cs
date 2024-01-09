using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{
    public class SmallDialogMessage
    {
        public Enum Result { get; set; }

        public SmallDialogMessage(Enum result)
        {
            Result = result;
        }
    }
}

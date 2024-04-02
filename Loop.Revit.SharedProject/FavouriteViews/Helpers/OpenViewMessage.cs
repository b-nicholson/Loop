using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public class OpenViewMessage
    {
        public ViewWrapper View{ get; set; }

        public OpenViewMessage(ViewWrapper view)
        {
            View = view;
        }
    }
}

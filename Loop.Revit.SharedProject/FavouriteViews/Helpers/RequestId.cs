using System;
using System.Collections.Generic;
using System.Text;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public enum RequestId
    {
        None,
        ActivateView,
        SwitchViewAndQueueClose,
        CloseOpenViews,
        CloseDocument,
        RefreshViews,
        RefreshTheme
    }
}

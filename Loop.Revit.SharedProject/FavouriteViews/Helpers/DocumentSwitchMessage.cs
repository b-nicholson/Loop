using System;
using Autodesk.Revit.DB;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public class DocumentSwitchMessage
    {
        public RequestId RequestId { get; set; }
        public Document Document { get; set; }
        public View View { get; set; }
        public DocumentSwitchMessage(RequestId requestId)
        {
            RequestId = requestId;
        }
    }
}

using Autodesk.Revit.UI;
using System;
using Autodesk.Revit.DB;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public enum RequestId
    {
        None,
        ActivateView
    }

    public class FavouriteViewsEventHandler : IExternalEventHandler
    {
        public RequestId Request { get; set; }
        public object Arg1 { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request)
                {
                    case RequestId.None:
                        return;
                    case RequestId.ActivateView:
                        ActivateView(app);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        public void ActivateView(UIApplication app)
        {
            var uiDoc = app.ActiveUIDocument;
            var viewWrapper = (ViewWrapper)Arg1;
            var view = (View)uiDoc.Document.GetElement(viewWrapper.ElementId);
            uiDoc.RequestViewChange(view);
        }

        public string GetName()
        {
            return "Favourite Views Event Handler";
        }
    }
}

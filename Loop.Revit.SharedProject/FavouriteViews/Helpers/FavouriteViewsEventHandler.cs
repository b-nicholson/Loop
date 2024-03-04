using Autodesk.Revit.UI;
using System;
using Autodesk.Revit.DB;

namespace Loop.Revit.FavouriteViews.Helpers
{
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
            var doc = viewWrapper.Document;
            var newUiDoc = new UIDocument(doc);

            var view = (View)newUiDoc.Document.GetElement(viewWrapper.ElementId);

            if (!Equals(uiDoc, newUiDoc))
            {
                //Select an element, and use that to force the active ui application to switch. Can't request a view change if its not the ActiveUiDoc.
                //No native method to switch em.
                var singleElem = new FilteredElementCollector(doc, viewWrapper.ElementId).WhereElementIsNotElementType().FirstElementId();
                newUiDoc.ShowElements(singleElem);
                newUiDoc.RefreshActiveView();

                newUiDoc.RequestViewChange(view);
            }
            else
            {
                newUiDoc.RequestViewChange(view);
            }
        }

        public string GetName()
        {
            return "Favourite Views Event Handler";
        }
    }
}

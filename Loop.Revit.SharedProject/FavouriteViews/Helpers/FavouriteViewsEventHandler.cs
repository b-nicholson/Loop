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

            //We have an override method for doc equals, but not uiDoc
            if (!Equals(uiDoc.Document, newUiDoc.Document))
            {
                //Select an element, and use that to force the active ui application to switch. Can't request a view change if its not the ActiveUiDoc.
                //No native method to switch em.

                //First try and only grab one element in the view, most efficient
                var singleElem = new FilteredElementCollector(doc, viewWrapper.ElementId).WhereElementIsNotElementType().FirstElementId();
                newUiDoc.ShowElements(singleElem);
                newUiDoc.RefreshActiveView();
                try
                {
                    newUiDoc.RequestViewChange(view);
                }
                catch (Exception e)
                {
                    //Then try grabbing all elements visible in the view, not awesome
                    var allElem = new FilteredElementCollector(doc, viewWrapper.ElementId).WhereElementIsNotElementType().ToElementIds();
                    newUiDoc.ShowElements(allElem);
                    newUiDoc.RefreshActiveView();
                    try
                    {
                        newUiDoc.RequestViewChange(view);
                    }
                    catch (Exception)
                    {
                        // at this point its an empty view, need to find a view with SOMETHING. 
                        var views = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views)
                            .ToElementIds();

                        foreach (var viewId in views)
                        {
                            var elems = new FilteredElementCollector(doc, viewId).WhereElementIsNotElementType().ToElementIds();
                            newUiDoc.ShowElements(elems);
                            newUiDoc.RefreshActiveView();
                            try
                            {
                                newUiDoc.RequestViewChange(view);
                                break;
                            }
                            catch (Exception)
                            {
                                //Do nothing, go to next view
                            }
                        }
                    }
                }
                
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

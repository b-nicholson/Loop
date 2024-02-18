using Autodesk.Revit.DB;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public class ViewActivatedMessage
    {
        public View NewView { get; set; }

        public Document Doc { get; set; }

        public ViewActivatedMessage(View newView, Document doc)
        {
            NewView = newView;
            Doc = doc;
        }
    }
}

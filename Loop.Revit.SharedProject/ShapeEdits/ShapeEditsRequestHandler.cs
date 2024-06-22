using Autodesk.Revit.UI;
using Loop.Revit.ViewTitles.Helpers;

namespace Loop.Revit.ShapeEdits
{
    public enum RequestId
    {
        None,
        Project
    }
    public class ShapeEditsRequestHandler : IExternalEventHandler
    {
        public RequestId Request { get; set; }
        public ShapeEditsModel Model { get; set; }
        public object Arg1 { get; set; }
        public object Arg2 { get; set; }
        private bool Cancel { get; set; }

        public void Execute(UIApplication app)
        {
            try
            {
                switch (Request)
                {
                    case RequestId.None:
                        return;
                    case RequestId.Project:
                        AdjustViewTitles(app);
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

        private void AdjustViewTitles(UIApplication app)
        {
            //TODO implement

        }

        public string GetName()
        {
            return "Shape Edits Event Handler";
        }

        private void OnCancel(CancelMessage obj)
        {
            Cancel = obj.Cancel;
        }

    }
}

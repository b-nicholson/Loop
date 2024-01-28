using System;
using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Loop.Revit.Utilities.Wpf.OutputListDialog
{
    public enum RequestId
    {
        None,
        FindElements
    }
    public class OutputListDialogEventHandler: IExternalEventHandler
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
                    case RequestId.FindElements:
                        FindElements(app);
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

        public void FindElements(UIApplication app)
        {
            var uiDoc = app.ActiveUIDocument;
            var elementId = (ElementId)Arg1;
            try
            {
                uiDoc.ShowElements(elementId);
                var coll = new Collection<ElementId>();
                coll.Add(elementId);
                uiDoc.Selection.SetElementIds(coll);
            }
            catch
            {
                //pass
            }
        }

        public string GetName()
        {
            return "Output List Dialog Event Handler";
        }
    }
}

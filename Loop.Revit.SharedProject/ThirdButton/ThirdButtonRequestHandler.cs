using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using GalaSoft.MvvmLight.Messaging;

namespace Loop.Revit.ThirdButton
{
    public enum RequestId
    {
        None,
        Delete
    }
    public class ThirdButtonRequestHandler : IExternalEventHandler
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
                    case RequestId.Delete:
                        Delete(app);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
               // ignore
            }
        }

        public void Delete(UIApplication app)
        {
            if (!(Arg1 is List<SpatialObjectWrapper> selected))
            {
                return;
            }
            var Doc = app.ActiveUIDocument.Document;
            var ids = selected.Select(x => x.Id).ToList();
            using (var trans = new Transaction(Doc, "Delete Rooms"))
            {
                trans.Start();
                Doc.Delete(ids);
                trans.Commit();
            }

            Messenger.Default.Send(new SpatialObjectDeletedMessage(ids));
        }

        public string GetName()
        {
            return "Third Button Request Handler";
        }
    }
}

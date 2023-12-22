using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using Autodesk.Revit.UI;

namespace Loop.Revit.ViewTitles
{
    public enum RequestId
    {
        None,
        AdjustViewTitleLengths
    }
    public class ViewTitlesRequestHandler : IExternalEventHandler
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
                    case RequestId.AdjustViewTitleLengths:
                        AdjustViewTitles(app);
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

        public void AdjustViewTitles(UIApplication app)
        {

            if (!(Arg1 is List<SheetWrapper> selected))
            {
                return;
            }

            //var ids = selected.Select(x => x.ViewportIds);

            var ids = selected.SelectMany(sheet => sheet.ViewportIds).ToList();


            var Doc = app.ActiveUIDocument.Document;
            var viewports = new FilteredElementCollector(Doc, ids).OfCategory(BuiltInCategory.OST_Viewports).Cast<Viewport>();
            
            var t = new Transaction(Doc, "Change Viewport Label Line Length");
            t.Start();



            foreach (var vp in viewports)
            {
                vp.LabelLineLength = 0.001;
                var rotation = vp.Rotation;


                if (rotation != ViewportRotation.None)
                    vp.Rotation = ViewportRotation.None;

                var outlines = vp.GetLabelOutline();
                var outlineMax = outlines.MaximumPoint;
                var outlineMin = outlines.MinimumPoint;

                var labelOffset = vp.LabelOffset;
                var boxMinPoint = vp.GetBoxOutline().MinimumPoint;
                var newPoint = (boxMinPoint + labelOffset).X;
                var symbolSize = newPoint - outlineMin.X;

                var userValue = 0.0;
                var userPadding = UnitUtils.ConvertToInternalUnits(userValue, UnitTypeId.Millimeters);

                //Add user padding
                var length = Math.Max(outlineMax.X - outlineMin.X, outlineMax.Y - outlineMin.Y) - symbolSize + userPadding;

                vp.LabelLineLength = length;
                if (rotation != ViewportRotation.None)
                    vp.Rotation = rotation;
            }


            t.Commit();
        }


        public string GetName()
        {
            return "View Titles Event Handler";
        }
    }
}

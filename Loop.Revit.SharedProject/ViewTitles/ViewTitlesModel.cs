using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;

namespace Loop.Revit.ViewTitles
{
    public class ViewTitlesModel
    {
        public UIApplication UiApp { get; }
        public Document Doc { get; }
        public ViewTitlesModel(UIApplication uiApp)
        {
            UiApp = uiApp;
            Doc = uiApp.ActiveUIDocument.Document;
        }

        public ObservableCollection<SheetWrapper> CollectSheets()
        {
            var sheets = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Sheets).Cast<ViewSheet>().Select(x => new SheetWrapper(x));
            return new ObservableCollection<SheetWrapper>(sheets);
        }

        public void ChangeTitleLength()
        {
            var viewports = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Viewports).Cast<Viewport>();

            var t = new Transaction(Doc, "Change Viewport Label Line Length");
            t.Start();

            foreach (var vp in viewports)
            {
                vp.LabelLineLength = 0.01;
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

                var userValue = 10.0;
                var userPadding = UnitUtils.ConvertToInternalUnits(userValue, UnitTypeId.Millimeters);

                //Add user padding
                var length = Math.Max(outlineMax.X - outlineMin.X, outlineMax.Y - outlineMin.Y) - symbolSize + userPadding;

                vp.LabelLineLength = length;
                if (rotation != ViewportRotation.None)
                    vp.Rotation = rotation;
            }
            

            t.Commit();

        }
    }
}

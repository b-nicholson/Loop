using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Autodesk.Revit.DB;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.ViewTitles.Helpers
{
    public class ViewportWrapper
    {
        public string SheetNumber { get; set; }
        public string SheetName { get; set; }
        public string ViewName { get; set; }
        public string TitleOnSheet { get; set; }

        public string Owner { get; set; }

        public ElementId Id { get; set; }

        public bool TestThings { get; set; }

        public ViewportWrapper(Viewport viewport, string owner)
        {
            Id = viewport.Id;
            SheetName = viewport.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NAME).AsString();
            SheetNumber = viewport.get_Parameter(BuiltInParameter.VIEWPORT_SHEET_NUMBER).AsString();
            ViewName = viewport.get_Parameter(BuiltInParameter.VIEW_NAME).AsString();
            TitleOnSheet = viewport.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION).AsString();
            Owner = owner;
        }
    }
}

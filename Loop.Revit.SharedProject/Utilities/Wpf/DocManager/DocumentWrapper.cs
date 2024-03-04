using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public class DocumentWrapper
    {
        public Document Doc { get; set; }

        public Color Color { get; set; }

        public DocumentWrapper(Document doc, Color color)
        {
            Doc = doc;
            Color = color;
        }
    }
}

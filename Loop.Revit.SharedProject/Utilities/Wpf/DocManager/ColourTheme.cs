using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public class ColourTheme
    {
        public Color Color { get; set; }
        public bool IsTaken { get; set; }

        public ColourTheme(Color color)
        {
            Color = color;
        }
    }
}

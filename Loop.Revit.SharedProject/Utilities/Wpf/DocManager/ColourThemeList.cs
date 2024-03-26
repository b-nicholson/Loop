using System.Collections.Generic;
using System.Windows.Media;

namespace Loop.Revit.Utilities.Wpf.DocManager
{
    public static class ColourThemeList
    {
        public static List<ColourTheme> Colours {get; set;} = new List<ColourTheme> { new ColourTheme(Colors.Red), new ColourTheme(Colors.Aqua), new ColourTheme(Colors.Green) };
    }
}

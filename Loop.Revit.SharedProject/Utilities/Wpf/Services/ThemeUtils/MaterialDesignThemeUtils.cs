using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Xml;

namespace Loop.Revit.Utilities.Wpf.Services.ThemeUtils
{
    public static class MaterialDesignThemeUtils
    {
        private static ITheme _theme { get; set; }
        public static ITheme GetMaterialDesignTheme(FrameworkElement xamlElement)
        {
            var resourceDictionaries = xamlElement.Resources.MergedDictionaries;
            foreach (var d in resourceDictionaries)
            {
                if (d is CustomColorTheme customTheme)
                {
                    _theme = d.GetTheme();
                    break;
                }
            }
            return _theme;
        }

        public static void SetMaterialDesignTheme(FrameworkElement xamlElement, ITheme theme)
        {
            _theme = theme;
            var resourceDictionaries = xamlElement.Resources.MergedDictionaries;
            foreach (var d in resourceDictionaries)
            {
                if (d is CustomColorTheme customTheme)
                {
                    d.SetTheme(_theme);
                }
            }
        }

        public static void ToggleDarkMode(FrameworkElement xamlElement, bool darkMode)
        {
            var theme = GetMaterialDesignTheme(xamlElement);
            var backgroundColour = Colors.White;
            var backgroundColourDark = Color.FromRgb(59, 68, 83);

            if (darkMode)
            {
                theme.SetBaseTheme(Theme.Dark);
                backgroundColour = backgroundColourDark;
            }
            else
            {
                theme.SetBaseTheme(Theme.Light);
            }

            var brush = xamlElement.FindResource("MaterialDesignPaper") as SolidColorBrush;
            if (brush != null)
            {
                if (brush.IsFrozen)
                {
                    // Clone the brush to make a modifiable copy
                    brush = brush.Clone();
                    // Now modify the clone
                    brush.Color = backgroundColour;
                    // Replace the resource with the modifiable clone
                    xamlElement.Resources["MaterialDesignPaper"] = brush;
                }
                else
                {
                    // Modify the brush directly
                    brush.Color = backgroundColour;
                }
            }
            SetMaterialDesignTheme(xamlElement, theme);

        }
    }
}

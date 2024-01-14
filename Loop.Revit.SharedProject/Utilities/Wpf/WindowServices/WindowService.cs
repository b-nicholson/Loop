using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.WindowServices
{
    public class WindowService : IWindowService
    {
        private readonly Window _window;
        private ITheme _theme;

        public WindowService(Window window)
        {
            _window = window;
        }
        public void CloseWindow()
        {
            _window.Close();
        }

        public ITheme GetMaterialDesignTheme()
        {
            var resourceDictionaries = _window.Resources.MergedDictionaries;

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

        public void SetMaterialDesignTheme(ITheme theme)
        {
            _theme = theme;
            var resourceDictionaries = _window.Resources.MergedDictionaries;
            foreach (var d in resourceDictionaries)
            {
                if (d is CustomColorTheme customTheme)
                {
                    d.SetTheme(_theme);
                }
            }
        }

        public void ToggleDarkMode(bool darkMode)
        {
            var theme = GetMaterialDesignTheme();
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

            var brush = _window.FindResource("MaterialDesignPaper") as SolidColorBrush;
            if (brush != null)
            {
                if (brush.IsFrozen)
                {
                    // Clone the brush to make a modifiable copy
                    brush = brush.Clone();
                    // Now modify the clone
                    brush.Color = backgroundColour;
                    // Replace the resource with the modifiable clone
                    _window.Resources["MaterialDesignPaper"] = brush;
                }
                else
                {
                    // Modify the brush directly
                    brush.Color = backgroundColour;
                }
            }
            SetMaterialDesignTheme(theme);

        }
    }
}

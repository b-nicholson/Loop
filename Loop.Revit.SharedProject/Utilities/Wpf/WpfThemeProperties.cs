using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf
{
    public static class WpfThemeProperties
    {
        public static readonly DependencyProperty ToggleThemeProperty = DependencyProperty.RegisterAttached(
            "ToggleTheme",
            typeof(bool),
            typeof(WpfThemeProperties),
            new PropertyMetadata(false, OnToggleThemeChanged));

        public static void SetToggleTheme(UIElement element, bool value)
        {
            element.SetValue(ToggleThemeProperty, value);
        }

        public static bool GetToggleTheme(UIElement element)
        {
            return (bool)element.GetValue(ToggleThemeProperty);
        }

        private static void OnToggleThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                ToggleTheme(window, (bool)e.NewValue);
            }
        }

        private static void ToggleTheme(Window window, bool? isDarkMode = null)
        {

            var mat = new CustomColorTheme();

            var colorAdjust = new ColorAdjustment();
            colorAdjust.DesiredContrastRatio = 4.5f;
            colorAdjust.Contrast = Contrast.Medium;
            colorAdjust.Colors = ColorSelection.All;

            
            var backgroundColour = Colors.White;
            var backgroundColourDark = Color.FromRgb(59, 68, 83);

            mat.BaseTheme = BaseTheme.Light;
            if (isDarkMode != null && (bool)isDarkMode)
            {
                mat.BaseTheme = BaseTheme.Dark;
                backgroundColour = backgroundColourDark;
            }

            mat.PrimaryColor = Color.FromRgb(56,66,189);
            mat.SecondaryColor = Color.FromRgb(156, 166, 89);
            mat.ColorAdjustment = colorAdjust;


            var brush = window.FindResource("MaterialDesignPaper") as SolidColorBrush;
            if (brush != null)
            {
                if (brush.IsFrozen)
                {
                    // Clone the brush to make a modifiable copy
                    brush = brush.Clone();
                    // Now modify the clone
                    brush.Color = backgroundColour;
                    // Replace the resource with the modifiable clone
                    window.Resources["MaterialDesignPaper"] = brush;
                }
                else
                {
                    // Modify the brush directly
                    brush.Color = backgroundColour;
                }
            }

            var resourceDictionaries = window.Resources.MergedDictionaries;

            List<ResourceDictionary> itemsToRemove = new List<ResourceDictionary>();

            // First, find the items to remove
            foreach (var d in resourceDictionaries)
            {
                if (d is CustomColorTheme customTheme)
                {
                    itemsToRemove.Add(d);
                }
            }

            // Then, modify the collection
            foreach (var item in itemsToRemove)
            {
                window.Resources.MergedDictionaries.Remove(item);
            }
            window.Resources.MergedDictionaries.Add(mat);
        }
    }

}

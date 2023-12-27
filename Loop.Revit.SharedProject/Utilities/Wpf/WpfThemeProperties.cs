using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using MaterialDesignColors;
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

        private static void ToggleTheme(Window window, bool isDarkMode)
        {

            var mat = new CustomColorTheme();
            mat.BaseTheme = BaseTheme.Light;
            if (isDarkMode)
            {
                mat.BaseTheme = BaseTheme.Dark;
            }

            mat.PrimaryColor = Color.FromRgb(56,66,189);
            mat.SecondaryColor = Color.FromRgb(156, 166, 89);


            var dicties = window.Resources.MergedDictionaries;

            List<ResourceDictionary> itemsToRemove = new List<ResourceDictionary>();

            // First, find the items to remove
            foreach (var d in dicties)
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

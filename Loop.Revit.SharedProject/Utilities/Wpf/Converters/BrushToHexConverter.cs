﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace Loop.Revit.Utilities.Wpf.Converters
{
    //Direct yoink
    //https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/MainDemo.Wpf/Converters/BrushToHexConverter.cs
    public class BrushToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string lowerHexString(int i) => i.ToString("X2").ToLower();
            var brush = (SolidColorBrush)value;
            var hex = lowerHexString(brush.Color.R) +
                      lowerHexString(brush.Color.G) +
                      lowerHexString(brush.Color.B);
            return "#" + hex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

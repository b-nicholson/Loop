using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows.Media;
using System.Globalization;

namespace Loop.Revit.Utilities.Json
{
    public class JsonColorConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var colorString = reader.GetString();
            return ColorConverterFromString(colorString);
        }

        private static Color ColorConverterFromString(string colorString)
        {
            byte a = 255, r = 0, g = 0, b = 0;
            if (colorString.StartsWith("#"))
            {
                string hex = colorString.Substring(1);
                if (hex.Length == 6 || hex.Length == 8)
                {
                    int startIdx = (hex.Length == 8) ? 2 : 0;
                    a = (hex.Length == 8) ? byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber) : (byte)255;
                    r = byte.Parse(hex.Substring(startIdx, 2), NumberStyles.HexNumber);
                    g = byte.Parse(hex.Substring(startIdx + 2, 2), NumberStyles.HexNumber);
                    b = byte.Parse(hex.Substring(startIdx + 4, 2), NumberStyles.HexNumber);
                }
            }
            return Color.FromArgb(a, r, g, b);
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            string colorString = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
            writer.WriteStringValue(colorString);
        }
    }
}

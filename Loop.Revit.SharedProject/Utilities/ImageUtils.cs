using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

namespace Loop.Revit.Utilities
{
    public static class ImageUtils
    {
        public static BitmapImage LoadImage(Assembly a, string name)
        {
            var img = new BitmapImage();
            try
            {
                var resourceName = a.GetManifestResourceNames().FirstOrDefault(x => x.Contains(name));
                var stream = a.GetManifestResourceStream(resourceName);

                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();

            }
            catch (Exception e)
            {
                //ignored
            }
            return img;
        }
    }
}

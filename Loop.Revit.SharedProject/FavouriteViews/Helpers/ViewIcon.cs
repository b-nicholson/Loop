using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using Loop.Revit.Utilities;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public class ViewIcon
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        public ViewType ViewType { get; set; }
        private string _lightImagePath;
        public string LightImagePath
        {
            get => _lightImagePath;
            set
            {
                LightBitmapImage = ImageUtils.LoadImage(_assembly, value);
                _lightImagePath = value;
            }
        }
        private string _darkImagePath;
        public string DarkImagePath
        {
            get => _darkImagePath;
            set
            {
                DarkBitmapImage = ImageUtils.LoadImage(_assembly, value);
                _darkImagePath = value;
            }
        }
        public BitmapImage LightBitmapImage { get; set; }
        public BitmapImage DarkBitmapImage { get; set; }
        public ViewIcon(ViewType viewType, string lightImagePath, string darkImagePath)
        {
            ViewType = viewType;
            LightImagePath = lightImagePath;
            DarkImagePath = darkImagePath;
        }
    }
}

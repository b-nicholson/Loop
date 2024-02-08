using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.ComponentModel;
using Loop.Revit.Utilities;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public class ViewIcon:ObservableObject
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
        private BitmapImage _lightBitmapImage;

        public BitmapImage LightBitmapImage
        {
            get => _lightBitmapImage; 
            set => SetProperty(ref _lightBitmapImage, value);
        }

        private BitmapImage _darkBitmapImage;

        public BitmapImage DarkBitmapImage
        {
            get => _darkBitmapImage; 
            set => SetProperty(ref _darkBitmapImage, value);
        }
        public ViewIcon(ViewType viewType, string lightImagePath, string darkImagePath)
        {
            ViewType = viewType;
            LightImagePath = lightImagePath;
            DarkImagePath = darkImagePath;
        }
    }
}

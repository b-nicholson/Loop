using Autodesk.Revit.UI;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Loop.Revit.Utilities.RevitUi
{
    public class CustomRibbonButton
    {
        private readonly Assembly _assembly = Assembly.GetExecutingAssembly();
        public PushButtonData Data { get; set; }
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
        public CustomRibbonButton(PushButtonData data, string lightImagePath, string darkImagePath)
        {
            Data = data;
            LightImagePath = lightImagePath;
            DarkImagePath = darkImagePath;
        }
    }
}

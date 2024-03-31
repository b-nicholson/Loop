using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;

namespace Utilities.Wpf.Services.WindowServices
{
    public class CustomColorThemeForMaterialDesign : ResourceDictionary
    {
        private Color _primaryColor;
        public Color PrimaryColor
        {
            get => _primaryColor;
            set
            {
                _primaryColor = value;
                UpdateTheme();
            }
        }

        private Color _secondaryColor;
        public Color SecondaryColor
        {
            get => _secondaryColor;
            set
            {
                _secondaryColor = value;
                UpdateTheme();
            }
        }

        private ITheme _customTheme;

        public ITheme CustomTheme
        {
            get => _customTheme;
            set
            {
                _customTheme = value;
            }

        }

        private void UpdateTheme()
        {
            var colorAdjust = new ColorAdjustment();
            colorAdjust.DesiredContrastRatio = 4.5f;
            colorAdjust.Contrast = Contrast.Medium;
            colorAdjust.Colors = ColorSelection.All;

            var nt = Theme.Create(Theme.Light, PrimaryColor, SecondaryColor);
            nt.ColorAdjustment = colorAdjust;
            CustomTheme = nt;


        }


    }
}

using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Utilities.Wpf.Services.PageServices
{
    public interface IPageService
    {
        Page GetPage();
        ITheme GetMaterialDesignTheme();
        void SetMaterialDesignTheme(ITheme materialDesignTheme);
        void ToggleDarkMode(bool darkMode);
    }
}

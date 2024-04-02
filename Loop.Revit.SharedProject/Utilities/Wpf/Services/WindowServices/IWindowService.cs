using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Utilities.Wpf.Services.WindowServices
{
    public interface IWindowService
    {
        void CloseWindow();

        Window GetWindow();
        ITheme GetMaterialDesignTheme();
        void SetMaterialDesignTheme(ITheme materialDesignTheme);

        void ToggleDarkMode(bool darkMode);
    }
}

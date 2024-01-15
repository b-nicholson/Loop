using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Loop.Revit.Utilities.Wpf.WindowServices
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

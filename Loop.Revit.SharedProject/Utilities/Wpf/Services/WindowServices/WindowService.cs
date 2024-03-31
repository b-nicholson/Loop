using System.Windows;
using System.Windows.Media;
using Loop.Revit.Utilities.Wpf.Services.ThemeUtils;
using MaterialDesignThemes.Wpf;

namespace Utilities.Wpf.Services.WindowServices
{
    public class WindowService : IWindowService
    {
        private readonly Window _window;
        private ITheme _theme;

        public WindowService(Window window)
        {
            _window = window;
        }
        public void CloseWindow()
        {
            _window.Close();
        }

        public Window GetWindow()
        {
            return _window;
        }

        public ITheme GetMaterialDesignTheme()
        {
            return MaterialDesignThemeUtils.GetMaterialDesignTheme(_window);
        }

        public void SetMaterialDesignTheme(ITheme theme)
        {
            MaterialDesignThemeUtils.SetMaterialDesignTheme(_window, theme);
        }

        public void ToggleDarkMode(bool darkMode)
        {
            MaterialDesignThemeUtils.ToggleDarkMode(_window, darkMode);
        }
    }
}

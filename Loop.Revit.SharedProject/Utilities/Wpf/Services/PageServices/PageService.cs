using System.Windows.Controls;
using System.Windows.Media;
using Loop.Revit.Utilities.Wpf.Services.ThemeUtils;
using MaterialDesignThemes.Wpf;

namespace Utilities.Wpf.Services.PageServices
{
    public class PageService : IPageService
    {
        private readonly Page _page;
        private ITheme _theme;

        public PageService(Page page)
        {
            _page = page;
        }

        public Page GetPage()
        {
            return _page;
        }

        public ITheme GetMaterialDesignTheme()
        {
            return MaterialDesignThemeUtils.GetMaterialDesignTheme(_page);
        }

        public void SetMaterialDesignTheme(ITheme theme)
        {
            MaterialDesignThemeUtils.SetMaterialDesignTheme(_page, theme);
        }

        public void ToggleDarkMode(bool darkMode)
        {
            MaterialDesignThemeUtils.ToggleDarkMode(_page, darkMode);
        }
    }
}


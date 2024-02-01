
namespace Loop.Revit.Utilities.UserSettings
{
    public class GlobalSettings
    {
        private static UserSetting _settings;

        public static UserSetting Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new UserSetting();
                }
                return _settings;
            }
            set => _settings = value;
        }
    }
}

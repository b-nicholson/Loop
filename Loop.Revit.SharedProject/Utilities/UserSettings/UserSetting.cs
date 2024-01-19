
using System.Windows.Media;

namespace Loop.Revit.Utilities.UserSettings
{
    public class UserSetting
    {
        public string Username { get; set; } = "DefaultUsername";
        public int Age { get; set; } = 25;
        public bool IsSubscribed { get; set; } = false;
        public bool IsDarkModeTheme { get; set; } = false;

        public Color PrimaryThemeColor { get; set; } = Color.FromRgb(0, 107, 255);

        public string AppVersion { get; set; } = "0.1";

        public override bool Equals(object obj)
        {
            return Equals(obj as UserSetting);
        }

        private bool Equals(UserSetting other)
        {
            if (other == null)
            {
                return false;
            }

            var type = GetType();
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var thisValue = property.GetValue(this);
                var otherValue = property.GetValue(other);
                if (!Equals(thisValue, otherValue))
                {
                    return false;
                }
            }
            return true;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                foreach (var property in GetType().GetProperties())
                {
                    var value = property.GetValue(this);
                    hash = hash * 23 + (value != null ? value.GetHashCode() : 0);
                }
                return hash;
            }
        }







    }
}

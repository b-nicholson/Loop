using System.Collections.Generic;

namespace Loop.Revit.Utilities.RevitUi
{
    public class RibbonButtonRecord
    {
        private static List<CustomRibbonButton> _customButtons;
        public static List<CustomRibbonButton> CustomButtons
        {
            get
            {
                if (_customButtons != null) return _customButtons;
                _customButtons = new List<CustomRibbonButton>();
                return _customButtons;
            }
            set => _customButtons = value;
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.UI;

namespace Loop.Revit.Utilities.RevitUi
{
    public class ChangeRibbonButtons
    {
        public static void SwapIconTheme(List<RibbonPanel> CustomPanels, UITheme theme)
        {
            foreach (var panel in CustomPanels)
            {
                var items = panel.GetItems();
                foreach (var item in items)
                {
                    if (!(item is RibbonButton button)) continue;
                    var buttonName = button.Name;
                    var buttons = RibbonButtonRecord.CustomButtons;
                    var matchedItem = buttons.FirstOrDefault(customButton => customButton.Data.Name == buttonName);
                    if (matchedItem != null)
                    {
                        button.LargeImage = theme == UITheme.Light ? matchedItem.LightBitmapImage : matchedItem.DarkBitmapImage;
                    }
                }
            }
        }
    }
}

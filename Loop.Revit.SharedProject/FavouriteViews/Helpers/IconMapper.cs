using Autodesk.Revit.DB;

namespace Loop.Revit.FavouriteViews.Helpers
{
    public static class IconMapper
    {
        public static ViewIcon GetIcon(View view)
        {
            var viewType = view.ViewType;
            var directory = "AdskIcons.";
            var lightPath = "";
            var darkPath = "";

            switch (viewType)
            {
                case ViewType.AreaPlan:
                    lightPath = "AreaPlan_light.ico";
                    darkPath = "AreaPlan_dark.ico";
                    break;
                case ViewType.CeilingPlan:
                    lightPath = "RCP_light.ico";
                    darkPath = "RCP_dark.ico";
                    break;
                case ViewType.ColumnSchedule:
                    lightPath = "ColumnSchedule_light.ico";
                    darkPath = "ColumnSchedule_dark.ico";
                    break;
                case ViewType.Detail:
                    lightPath = "Callout_light.ico";
                    darkPath = "Callout_dark.ico";
                    break;
                case ViewType.DraftingView:
                    lightPath = "Drafting_light.ico";
                    darkPath = "Drafting_dark.ico";
                    break;
                case ViewType.DrawingSheet:
                    lightPath = "Sheet_light.ico";
                    darkPath = "Sheet_dark.ico";
                    break;
                case ViewType.Elevation:
                    lightPath = "Elevation_light.ico";
                    darkPath = "Elevation_dark.ico";
                    break;
                case ViewType.EngineeringPlan:
                    lightPath = "StructuralPlan_light.ico";
                    darkPath = "StructuralPlan_dark.ico";
                    break;
                case ViewType.FloorPlan:
                    lightPath = "FloorPlan_light.ico";
                    darkPath = "FloorPlan_dark.ico";
                    break;
                case ViewType.Legend:
                    lightPath = "Legend_light.ico";
                    darkPath = "Legend_dark.ico";
                    break;
                case ViewType.PanelSchedule:
                    lightPath = "PanelSchedule_light.ico";
                    darkPath = "PanelSchedule_dark.ico";
                    break;
                case ViewType.Rendering:
                    lightPath = "Render_light.ico";
                    darkPath = "Render_dark.ico";
                    break;
                case ViewType.Section:
                    lightPath = "Section_light.ico";
                    darkPath = "Section_dark.ico";
                    break;
                case ViewType.Schedule:
                    lightPath = "Schedule_light.ico";
                    darkPath = "Schedule_dark.ico";
                    break;
                case ViewType.ThreeD:
                    lightPath = "3D_light.ico";
                    darkPath = "3D_dark.ico";
                    break;
                case ViewType.Walkthrough:
                    lightPath = "Walkthrough_light.ico";
                    darkPath = "Walkthrough_dark.ico";
                    break;
                default:
                    lightPath = "Report_light.ico";
                    darkPath = "Report_dark.ico";
                    break;
            }

            return new ViewIcon(viewType, directory+lightPath, directory + darkPath);
        }
    }
}

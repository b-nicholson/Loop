using CommunityToolkit.Mvvm.ComponentModel;
using Utilities.Wpf.Services.WindowServices;

namespace Loop.Revit.ShapeEdits
{
    public class ShapeEditsViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ShapeEditsModel _model;

        public ShapeEditsViewModel(ShapeEditsModel model, IWindowService windowService)
        {
            _model = model;
            _windowService = windowService;
        }
    }
}

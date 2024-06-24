using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Windows.Documents;
using Autodesk.Revit.DB;
using CommunityToolkit.Mvvm.Messaging;
using Loop.Revit.ShapeEdits.Helpers;
using Loop.Revit.ViewTitles;
using Loop.Revit.ViewTitles.Helpers;
using Utilities.Wpf.Services.WindowServices;

namespace Loop.Revit.ShapeEdits
{
    public class ShapeEditsViewModel : ObservableObject
    {
        private readonly IWindowService _windowService;
        private readonly ShapeEditsModel _model;
        public RelayCommand Run { get; set; }
        public RelayCommand SelectTarget { get; set; }
        public RelayCommand SelectHost { get; set; }

        private List<Element> _selectedTargets;

        public List<Element> SelectedTargets
        {
            get => _selectedTargets;
            set => SetProperty(ref _selectedTargets, value);
        }

        private List<Element> _hostElements;
        public List<Element> HostElements
        {
            get => _hostElements;
            set => SetProperty(ref _hostElements, value);
        }

        public ShapeEditsViewModel(ShapeEditsModel model, IWindowService windowService)
        {
            _model = model;
            _windowService = windowService;

            Run = new RelayCommand(OnRun);
            SelectTarget = new RelayCommand(OnSelectTarget);
            SelectHost = new RelayCommand(OnSelectHost);

            WeakReferenceMessenger.Default.Register<ShapeEditsViewModel, ShapeEditsSelectionMessage>(this, (r, m) => r.OnSelectionUpdate(m));
        }

        private void OnSelectionUpdate(ShapeEditsSelectionMessage m)
        {
            if (m.IsTargetElement)
            {
                SelectedTargets = m.SelectedElements;
            }
            else
            {
                HostElements = m.SelectedElements;
            }
        }

        private void OnRun()
        {
            _model.Project(SelectedTargets, HostElements, false, false);
        }

        private void OnSelectTarget()
        {
            _model.SelectObjects(true);

        }

        private void OnSelectHost()
        {
            _model.SelectObjects(false);
        }
    }
}

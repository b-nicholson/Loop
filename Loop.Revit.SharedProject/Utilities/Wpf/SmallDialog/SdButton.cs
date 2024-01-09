using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ControlzEx.Standard;

namespace Loop.Revit.Utilities.Wpf.SmallDialog
{
    public class SdButton: ObservableObject
    {
        private string _name;

        public string Name
        {
            get=> _name;
            set => SetProperty(ref _name, value);
        }

        private Enum _results;
        public Enum Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }

        public SdButton(string name, Enum results)
        {
            Name = name;
            Results = results;
        }
    }
}

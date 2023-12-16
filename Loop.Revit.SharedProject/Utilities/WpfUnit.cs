using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Utilities;

namespace Loop.Revit.Utilities
{
    public class WpfUnit: INotifyPropertyChanged
    {
        public string InputUnits { get; set; }

        public RevitUnit Units { get; set; }


        public WpfUnit(string inputUnits, RevitUnit units)
        {
            InputUnits = inputUnits;
            Units = units;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

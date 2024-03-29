﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Loop.Revit.ViewTitles.Helpers
{
    public class PropertyWrapper : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public ObservableCollection<PropertyInfo> Value { get; set; }

        public PropertyWrapper(string name, ObservableCollection<PropertyInfo> value)
        {
            Name = Regex.Replace(name, "[a-z][A-Z]", m => $"{m.Value[0]} {m.Value[1]}");

            Value = value;

        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}

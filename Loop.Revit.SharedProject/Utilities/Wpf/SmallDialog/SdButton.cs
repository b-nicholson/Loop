﻿using System;
using CommunityToolkit.Mvvm.ComponentModel;

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

        private SmallDialogResults _results;
        public SmallDialogResults Results
        {
            get => _results;
            set => SetProperty(ref _results, value);
        }

        public SdButton(string name, SmallDialogResults results)
        {
            Name = name;
            Results = results;
        }
    }
}

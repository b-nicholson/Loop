using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Loop.Revit.ViewTitles
{
    class ViewTitlesValidator: AbstractValidator<ViewTitlesViewModel>
    {
        public ViewTitlesValidator()
        {
            RuleFor(s => s.SelectedSheetCount).GreaterThan(0);
            
        }

    }
}

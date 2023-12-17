using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace Loop.Revit.Utilities
{
    public class WpfRevitUnitValidator: AbstractValidator<WpfUnit>
    {
        public WpfRevitUnitValidator()
        {
            RuleFor(u => u.InputUnits)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} cannot be empty")
                .Length(5,54).WithMessage("Please input a valid {PropertyName}")
                .Must(BeAValidUnit).WithMessage("{PropertyName} must be a number");
        }

        protected bool BeAValidUnit(string unitValue)
        {
            bool valid;
            try
            {
                var val = Convert.ToDouble(unitValue);
                valid = true;
            }
            catch
            {
                valid = false;
            }

            return valid;
        }
    }
}

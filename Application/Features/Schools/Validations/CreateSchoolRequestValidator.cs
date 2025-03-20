using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Validations
{
    internal class CreateSchoolRequestValidator : AbstractValidator<CreateSchoolRequest>
    {
        public CreateSchoolRequestValidator()
        {
            RuleFor(req => req.Name)
                 .NotEmpty().WithMessage("Name is required.")
                 .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
            RuleFor(req => req.EstablishedDate)
                .NotEmpty().WithMessage("Established date is required.")
                .LessThan(DateTime.Now).WithMessage("Established date must be less than today.");
           
        }
    }
}

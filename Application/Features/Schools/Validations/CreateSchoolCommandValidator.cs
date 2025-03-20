using Application.Features.Schools.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Validations
{
    public class CreateSchoolCommandValidator : AbstractValidator<CreateSchoolCommand>
    {
        public CreateSchoolCommandValidator()
        {
            RuleFor(req => req.CreateSchool)
                 .SetValidator(new CreateSchoolRequestValidator());
        }
    }
}

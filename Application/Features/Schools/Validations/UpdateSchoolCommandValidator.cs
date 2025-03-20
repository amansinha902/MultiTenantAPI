using Application.Features.Schools.Commands;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Validations
{
    public class UpdateSchoolCommandtValidator : AbstractValidator<UpdateSchoolCommand>
    {
        public UpdateSchoolCommandtValidator(ISchoolService schoolService)
        {
           RuleFor(req => req.MyProperty)
                .SetValidator(new UpdateSchoolRequestValidator(schoolService));
        }
    }
}

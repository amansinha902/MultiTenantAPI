using Domain.Entities;
using FluentValidation;

namespace Application.Features.Schools.Validations
{
    public class UpdateSchoolRequestValidator : AbstractValidator<UpdateSchoolRequest>
    {
        public UpdateSchoolRequestValidator(ISchoolService schoolService)
        {
            RuleFor(req => req.Id)
                .NotEmpty()
                 .MustAsync(async (id, token) => await schoolService.GetByIdAsync(id) is School schoolInDb && schoolInDb.Id == id)
                 .WithMessage("School does not exist.");
            RuleFor(req => req.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");
            RuleFor(req => req.EstablishedDate)
                .NotEmpty().WithMessage("Established date is required.")
                .LessThan(DateTime.Now).WithMessage("Established date must be less than today.");
        }
    }

}

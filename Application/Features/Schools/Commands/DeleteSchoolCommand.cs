using Application.Pipelines;
using Application.Wrappers;
using MediatR;

namespace Application.Features.Schools.Commands
{
    public class DeleteSchoolCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public int SchoolId { get; set; }
    }
    public class DeleteSchoolCommandHandler : IRequestHandler<DeleteSchoolCommand, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public DeleteSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(DeleteSchoolCommand request, CancellationToken cancellationToken)
        {
            var schoolInDb = await _schoolService.GetByIdAsync(request.SchoolId);
            if (schoolInDb is not null)
            {
               var deletedSchoolId  = await _schoolService.DeleteAsync(schoolInDb);
               return await ResponseWrapper<int>.SuccessAsync(data: deletedSchoolId, "School Deleted Successfully");
            }
            return await ResponseWrapper<int>.FailAsync("School Does Not Exist");
        }
    }
}

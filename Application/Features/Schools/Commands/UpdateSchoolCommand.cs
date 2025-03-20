using Application.Pipelines;
using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Commands
{
    public class UpdateSchoolCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public UpdateSchoolRequest MyProperty { get; set; }
    }
    public class UpdateSchoolCommandHandler : IRequestHandler<UpdateSchoolCommand, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public UpdateSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(UpdateSchoolCommand request, CancellationToken cancellationToken)
        {
            var schoolInDb = await _schoolService.GetByIdAsync(request.MyProperty.Id);
            if (schoolInDb is not null)
            {
                schoolInDb.Name = request.MyProperty.Name;
                schoolInDb.EstablishedDate = request.MyProperty.EstablishedDate;
                var updatedSchoolId = await _schoolService.UpdateAsync(schoolInDb);
                return await ResponseWrapper<int>.SuccessAsync(data: updatedSchoolId, "School Updated Succesfully");
            }
            return await ResponseWrapper<int>.FailAsync("School Does Not Exist");
        }
    }

}

using Application.Wrappers;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Query
{
    public class GetSchoolByIdQuery : IRequest<IResponseWrapper>
    {
        public int SchoolId { get; set; }
    }
    public class GetSchoolByIdQueryHandler : IRequestHandler<GetSchoolByIdQuery, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public GetSchoolByIdQueryHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(GetSchoolByIdQuery request, CancellationToken cancellationToken)
        {
            var schoolInDb = await _schoolService.GetByIdAsync(request.SchoolId);
            if (schoolInDb is not null) 
            {
                return await ResponseWrapper<SchoolResponse>.SuccessAsync(data: schoolInDb.Adapt<SchoolResponse>(), "School Fetched Succesfully");
            }
            return await ResponseWrapper<int>.FailAsync("School Does Not Exist");

        }
    }
}

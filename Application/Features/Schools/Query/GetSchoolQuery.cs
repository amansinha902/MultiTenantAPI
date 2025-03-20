using Application.Wrappers;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Query
{
    public class GetSchoolQuery : IRequest<IResponseWrapper> 
    {

    }
    public class GetSchoolQueryHandler : IRequestHandler<GetSchoolQuery, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public GetSchoolQueryHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(GetSchoolQuery request, CancellationToken cancellationToken)
        {
            var schoolInDb = await _schoolService.GetAllAsync();  
            if(schoolInDb is not null)
            {
                return  await ResponseWrapper<List<SchoolResponse>>.SuccessAsync(data: schoolInDb.Adapt<List<SchoolResponse>>(), "School Fetched Successfully");
            }
            return await ResponseWrapper<int>.FailAsync("No School Were Found!!");
        }
    }
}

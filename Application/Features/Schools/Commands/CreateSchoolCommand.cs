﻿using Application.Pipelines;
using Application.Wrappers;
using Domain.Entities;
using javax.xml.crypto;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Schools.Commands
{
    public class CreateSchoolCommand : IRequest<IResponseWrapper>, IValidateMe
    {
        public CreateSchoolRequest CreateSchool { get; set; }
    }
    public class CreateSchoolCommandHandler : IRequestHandler<CreateSchoolCommand, IResponseWrapper>
    {
        private readonly ISchoolService _schoolService;

        public CreateSchoolCommandHandler(ISchoolService schoolService)
        {
            _schoolService = schoolService;
        }

        public async Task<IResponseWrapper> Handle(CreateSchoolCommand request, CancellationToken cancellationToken)
        {
            var newSchool = request.CreateSchool.Adapt<School>();
            var schoolId = await _schoolService.CreateAsync(newSchool);
            return await ResponseWrapper<int>.SuccessAsync(data:schoolId,"School Created Succesfully");
        }
    }

}

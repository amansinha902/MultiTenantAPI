﻿using Application.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Tenancy.Commands
{
    public class ActivateTenantCommand : IRequest<IResponseWrapper>
    {
        public string TenantId { get; set; }
    }

    public class ActivateTenantCommandHandler : IRequestHandler<ActivateTenantCommand, IResponseWrapper>
    {
        private readonly ITenantService _tenantService;

        public ActivateTenantCommandHandler(ITenantService tenantService)
        {
            _tenantService = tenantService;
        }

        public async Task<IResponseWrapper> Handle(ActivateTenantCommand request, CancellationToken cancellationToken)
        {
            var tenantId = await _tenantService.ActivateAsync(request.TenantId);
            return await ResponseWrapper<string>.SuccessAsync(data: tenantId, "Tenant activation successful.");
        }
    }
}

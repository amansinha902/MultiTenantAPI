﻿using Application.Features.Tenancy;
using Application.Features.Tenancy.Commands;
using Application.Features.Tenancy.Queries;
using Infrastructure.Constants;
using Infrastructure.Identity.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TenantsController : BaseApiController
    {
        [HttpPost("add")]
        [ShouldHavePermission(SchoolConstants.Create, SchoolFeature.Tenants)]
        public async Task<IActionResult> CreateTenantAsync([FromBody] CreateTenantRequest createTenantRequest)
        {
            var response = await Sender.Send(new CreateTenantCommand { CreateTenant = createTenantRequest });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPut("{tenantId}/activate")]
        [ShouldHavePermission(SchoolConstants.Update, SchoolFeature.Tenants)]
        public async Task<IActionResult> ActivateTenantAsync(string tenantId)
        {
            var response = await Sender.Send(new ActivateTenantCommand { TenantId = tenantId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("{tenantId}/deactivate")]
        [ShouldHavePermission(SchoolConstants.Update, SchoolFeature.Tenants)]
        public async Task<IActionResult> DeactivateTenantAsync(string tenantId)
        {
            var response = await Sender.Send(new DeactivateTenantCommand { TenantId = tenantId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("upgrade")]
        [ShouldHavePermission(SchoolConstants.UpgradeSubscription, SchoolFeature.Tenants)]
        public async Task<IActionResult> UpgradeTenantSubscriptionAsync([FromBody] UpdateTenantSubscriptionRequest updateTenant)
        {
            var response = await Sender.Send(new UpdateTenantSubscriptionCommand { UpdateTenantSubscription = updateTenant });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("{tenantId}")]
        [ShouldHavePermission(SchoolConstants.Read, SchoolFeature.Tenants)]
        public async Task<IActionResult> GetTenantByIdAsync(string tenantId)
        {
            var response = await Sender.Send(new GetTenantByIdQuery { TenantId = tenantId });
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("all")]
        [ShouldHavePermission(SchoolConstants.Read, SchoolFeature.Tenants)]
        public async Task<IActionResult> GetTenantsAsync()
        {
            var response = await Sender.Send(new GetTenantsQuery());
            if (response.IsSuccessful)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}

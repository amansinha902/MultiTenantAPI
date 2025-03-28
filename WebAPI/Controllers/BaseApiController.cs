﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private ISender _sender = null;
        public ISender Sender => _sender ??= HttpContext.RequestServices.GetRequiredService<ISender>(); 

    }
}

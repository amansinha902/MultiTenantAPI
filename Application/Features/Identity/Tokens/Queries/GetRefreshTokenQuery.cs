﻿using Application.Wrappers;
using com.sun.org.apache.xml.@internal.resolver.helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Tokens.Queries
{
    public class GetRefreshTokenQuery :IRequest<IResponseWrapper>
    {
        public RefreshTokenRequest RefreshToken { get; set; }
    }
    public class GetRefreshTokenQueryHandler : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
    {
        private readonly ITokenService _tokenService;

        public GetRefreshTokenQueryHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            var refreshToken = await _tokenService.RefreshTokenAsync(request.RefreshToken);
            return await ResponseWrapper<TokenResponse>.SuccessAsync(refreshToken);
        }
    }   
}

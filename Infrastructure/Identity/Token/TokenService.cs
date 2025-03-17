using Application;
using Application.Exceptions;
using Application.Features.Identity.Tokens;
using Finbuckle.MultiTenant.Abstractions;
using Infrastructure.Constants;
using Infrastructure.Identity.Models;
using Infrastructure.Tenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Identity.Token
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtSettings _jwtSettings;

        private readonly IMultiTenantContextAccessor<ABSchoolTenantInfo> _tenantContextAccessor;
        public TokenService(UserManager<ApplicationUser> userManager, IMultiTenantContextAccessor<ABSchoolTenantInfo> tenantContextAccessor, RoleManager<ApplicationRole> roleManager, IOptions<JwtSettings> jwtSettings)
        {
            _userManager = userManager;
            _tenantContextAccessor = tenantContextAccessor;
            _roleManager = roleManager;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<TokenResponse> LoginAsync(TokenRequest request)
        {
            #region Validation
            if (!_tenantContextAccessor.MultiTenantContext.TenantInfo.IsActive)
            {
              throw new UnautorizeException(["Tenant is not active"]);
            }
            var userInDb = await _userManager.FindByNameAsync(request.UserName);
            if (userInDb == null)
            {
                throw new UnautorizeException(["Invalid username or password"]);
            }
            if(await _userManager.CheckPasswordAsync(userInDb, request.Password))
            {
                throw new UnautorizeException(["Invalid username or password"]);
            }
            if (!userInDb.IsActive)
            {
                throw new UnautorizeException(["User is not active"]);
            }
            if(_tenantContextAccessor.MultiTenantContext.TenantInfo.Id is not TenancyConstans.Root.Id)
            {
                if(_tenantContextAccessor.MultiTenantContext.TenantInfo.ValidUpto < DateTime.UtcNow)
                {
                    throw new UnautorizeException(["Subscription is Expired"]);
                }
            }
            #endregion
            #region GenerateToken
            return await GenerateTokenAndUpdateUserAsync(userInDb);
            #endregion
        }

        public async Task<TokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var userPrincipal = GetClaimPricipalFromExpiringToken(request.CurrentJwt);
            var userEmail = userPrincipal.GetEmail();
            var userInDb = await _userManager.FindByEmailAsync(userEmail) 
                ?? throw new UnautorizeException(["Invalid Token"]);
            if (userInDb.RefreshToken != request.CurrentRefreshToken || userInDb.RefreshTokenExpiryTime < DateTime.UtcNow)
            {
                throw new UnautorizeException(["Invalid Token"]);
            }
            return await GenerateTokenAndUpdateUserAsync(userInDb);
        }
        private ClaimsPrincipal GetClaimPricipalFromExpiringToken(string expiringToken)
        {
            var tkValidationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiringToken, tkValidationParams, out var validatedToken);
            if(validatedToken is not JwtSecurityToken jwtSecurityToken || 
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new UnautorizeException(["Invalid Token"]);
            }
            return principal;
        }
        private async Task<TokenResponse> GenerateTokenAndUpdateUserAsync(ApplicationUser user)
        {
            var newJwt = await GenerateJwtToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationTimeInDays);
            await _userManager.UpdateAsync(user);
            return new TokenResponse
            {
                Jwt = newJwt,
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryDate = user.RefreshTokenExpiryTime
            };

        }
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            return GenerateEncryptedToken(GenerateSigningCredentials(), await GetClaims(user));
        }
        private async Task<IEnumerable<Claim>> GetClaims(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var userRole in userRoles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, userRole));
                var currentRole = await _roleManager.FindByNameAsync(userRole);
                var allPermissions = await _roleManager.GetClaimsAsync(currentRole);
                permissionClaims.AddRange(allPermissions);
            }
            var claims = new List<Claim> { 
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
                new(ClaimConstants.Tenant, _tenantContextAccessor.MultiTenantContext.TenantInfo.Id)
            }.Union(roleClaims).Union(userClaims).Union(permissionClaims);

            return claims;
        }
        private SigningCredentials GenerateSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinute),
                signingCredentials: signingCredentials
                );
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        private string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
    
}

﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StolenVehicleLocatorSystem.Business.Interfaces
{
    public interface ITokenService
    {
        JwtSecurityToken CreateAccessToken(IList<Claim> authClaims);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromToken(string token, bool isExpired);
    }
}
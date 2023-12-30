﻿using API.Contracts;
using API.DTOs.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Utilities.Handler
{
    public class TokenHandler : ITokenHandlers
    {
        private readonly IConfiguration _configuration;

        public TokenHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Generate(IEnumerable<Claim> claims)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTService:SecretKey"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokenOptions = new JwtSecurityToken(issuer: _configuration["JWTService:Issuer"],
                audience: _configuration["JWTService:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: signingCredentials);
            var encodedToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return encodedToken;
        }

        public ClaimsDto ExtractClaimsFromJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new ClaimsDto();
            }

            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWTService:Audience"],
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWTService:Issuer"],
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWTService:SecretKey"]))
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out _);

                if (claimsPrincipal.Identity is ClaimsIdentity identity)
                {
                    var claims = new ClaimsDto
                    {
                        CompanyGuid = Guid.Parse(identity.FindFirst("CompanyGuid")?.Value ?? ""),
                        Name = identity.FindFirst("Name")?.Value,
                        Email = identity.FindFirst("Email")?.Value,
                        Foto = identity.FindFirst("Foto")?.Value,
                        StatusAccount = identity.FindFirst("StatusAccount")?.Value,
                        StatusVendor = identity.FindFirst("StatusVendor")?.Value,
                    };

                    var roles = identity.Claims.Where(c => c.Type == ClaimTypes.Role).Select(claim => claim.Value).ToList();
                    claims.Role = roles;

                    return claims;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error extracting claims from JWT: " + ex.Message);
            }

            return new ClaimsDto();
        }
    }
}

﻿using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CitiesManager.Core.Services
{
	public class JwtService : IJwtService
	{
		private readonly IConfiguration _configuration;

		public JwtService(IConfiguration configuration)
		{
			_configuration = configuration;
		}


		/// <summary>
		/// Generates a JWT token using the given user's information and the configuration settings.
		/// </summary>
		/// <param name="user">ApplicationUser object</param>
		/// <returns>AuthenticationResponse that includes token</returns>
		public AuthenticationResponse CreateJwtToken(ApplicationUser user)
		{
			DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));

			Claim[] claims = new Claim[] {
				 new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				 new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
				 new Claim(ClaimTypes.NameIdentifier, user.Email),
				 new Claim(ClaimTypes.Name, user.PersonName),
				 new Claim(ClaimTypes.Email, user.Email),
			};

			SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

			SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			JwtSecurityToken tokenGenerator = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: expiration,
				signingCredentials: signingCredentials
			);

			JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
			string token = tokenHandler.WriteToken(tokenGenerator);

			return new AuthenticationResponse()
			{
				Token = token,
				Email = user.Email,
				PersonName = user.PersonName,
				Expiration = expiration,
				RefreshToken = GenerateRefreshToken(),
				RefreshTokenExpiration = DateTime.Now.AddMinutes(Convert.ToInt32(_configuration["RefreshToken:EXPIRATION_MINUTES"]))
			};
		}

		private static string GenerateRefreshToken()
		{
			byte[] bytes = new byte[64];
			var randomNumberGenerator = RandomNumberGenerator.Create();
			randomNumberGenerator.GetBytes(bytes);
			return Convert.ToBase64String(bytes);
		}

		public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
		{
			var tokenValidationParameters = new TokenValidationParameters()
			{
				ValidateAudience = true,
				ValidAudience = _configuration["Jwt:Audience"],
				ValidateIssuer = true,
				ValidIssuer = _configuration["Jwt:Issuer"],
				ValidateLifetime = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
			};

			JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
			ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken
				|| !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}
	}
}
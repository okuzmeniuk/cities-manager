using CitiesManager.Core.DTO;
using CitiesManager.Core.Identity;
using CitiesManager.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CitiesManager.WebAPI.Controllers.v1
{
	/// <summary>
	/// 
	/// </summary>
	[AllowAnonymous]
	[ApiVersion("1.0")]
	public class AccountController : CustomControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<ApplicationRole> _roleManager;
		private readonly IJwtService _jwtService;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userManager"></param>
		/// <param name="signInManager"></param>
		/// <param name="roleManager"></param>
		/// <param name="jwtService"></param>
		public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, IJwtService jwtService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_jwtService = jwtService;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registerDTO"></param>
		/// <returns></returns>
		[HttpPost("register")]
		public async Task<ActionResult<ApplicationUser>> PostRegister(RegisterDTO registerDTO)
		{
			if (!ModelState.IsValid)
			{
				string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
				return Problem(errorMessage);
			}

			ApplicationUser user = new ApplicationUser()
			{
				Email = registerDTO.Email,
				PhoneNumber = registerDTO.PhoneNumber,
				UserName = registerDTO.Email,
				PersonName = registerDTO.PersonName
			};

			IdentityResult result = await _userManager.CreateAsync(user, registerDTO.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, false);

				var authenticationResponse = _jwtService.CreateJwtToken(user);
				user.RefreshToken = authenticationResponse.RefreshToken;
				user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;
				await _userManager.UpdateAsync(user);

				return Ok(authenticationResponse);
			}
			else
			{
				string errorMessage = string.Join(" | ", result.Errors.Select(e => e.Description));
				return Problem(errorMessage);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="email"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> IsEmailAlreadyRegistered(string email)
		{
			ApplicationUser? user = await _userManager.FindByEmailAsync(email);
			return Ok(user is null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="loginDTO"></param>
		/// <returns></returns>
		[HttpPost("login")]
		public async Task<IActionResult> PostLogin(LoginDTO loginDTO)
		{
			if (!ModelState.IsValid)
			{
				string errorMessage = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
				return Problem(errorMessage);
			}

			var result = await _signInManager.PasswordSignInAsync(loginDTO.Email, loginDTO.Password, isPersistent: false, lockoutOnFailure: false);

			if (result.Succeeded)
			{
				ApplicationUser? user = await _userManager.FindByEmailAsync(loginDTO.Email);

				if (user is null)
				{
					return NoContent();
				}

				await _signInManager.SignInAsync(user, false);

				var authenticationResponse = _jwtService.CreateJwtToken(user);
				user.RefreshToken = authenticationResponse.RefreshToken;
				user.RefreshTokenExpiration = authenticationResponse.RefreshTokenExpiration;
				await _userManager.UpdateAsync(user);

				return Ok(authenticationResponse);
			}

			return Problem("Invalid email or password");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		[HttpGet("logout")]
		public async Task<ActionResult<ApplicationUser>> GetLogout()
		{
			await _signInManager.SignOutAsync();
			return NoContent();
		}

		[HttpPost("generate-new-jwt-token")]
		public async Task<IActionResult> GenerateNewAccessToken(TokenModel tokenModel)
		{
			if (tokenModel is null)
			{
				return BadRequest("Invalid client request");
			}

			ClaimsPrincipal? principal = _jwtService.GetPrincipalFromJwtToken(tokenModel.Token);

			if (principal is null)
			{
				return BadRequest("Invalid jwt access token");
			}

			string? email = principal.FindFirstValue(ClaimTypes.Email);
			ApplicationUser? user = await _userManager.FindByEmailAsync(email);

			if (user is null || user.RefreshToken != tokenModel.RefreshToken || user.RefreshTokenExpiration <= DateTime.Now)
			{
				return BadRequest("Invalid refresh token");
			}

			AuthenticationResponse response = _jwtService.CreateJwtToken(user);
			user.RefreshToken = response.RefreshToken;
			user.RefreshTokenExpiration = response.RefreshTokenExpiration;
			await _userManager.UpdateAsync(user);

			return Ok(response);
		}
	}
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.DTO
{
	public class RegisterDTO
	{
		[Required(ErrorMessage = "Person Name cannot be blank")]
		public string PersonName { get; set; } = string.Empty;

		[Required(ErrorMessage = "Email cannot be blank")]
		[EmailAddress(ErrorMessage = "Email should be in a proper format")]
		[Remote(action: "IsEmailAlreadyRegistered", controller: "Account", ErrorMessage = "Email is already in use")]
		public string Email { get; set; } = string.Empty;

		[Required(ErrorMessage = "Phone number cannot be blank")]
		[RegularExpression(@"^[0-9]*$", ErrorMessage = "Phone number should be in a proper format")]
		//[Remote(action: "IsPhoneAlreadyRegistered", controller: "Account", ErrorMessage = "Phone number is already in use")]
		public string PhoneNumber { get; set; } = string.Empty;

		[Required(ErrorMessage = "Password cannot be blank")]
		public string Password { get; set; } = string.Empty;

		[Required(ErrorMessage = "Confirmation password cannot be blank")]
		[Compare("Password", ErrorMessage = "Password and confirm password must be same")]
		public string ConfirmPassword { get; set; } = string.Empty;
	}
}

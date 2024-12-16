using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
	public class RegisterViewModel
	{
		[Required(ErrorMessage ="First Name Is Required")]
		public string FirstName { get; set; }
		[Required(ErrorMessage = "Last Name Is Required")]
		public string LastName { get; set; }
		[Required(ErrorMessage = "Email Is Required")]
		[EmailAddress(ErrorMessage ="Invalid Email")]
		public string Email { get; set; }
		[Required(ErrorMessage = "Password Is Required")]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		[Required(ErrorMessage = "Confirm Password Is Required")]
		[DataType(DataType.Password)]
		[Compare("Password",ErrorMessage ="Password Don't Match")]
		public string ConfirmPassword { get; set; }
        public bool IsAgree { get; set; }
    }
}

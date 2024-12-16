using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager)
        {
			_userManager = userManager;
			_signInManager = signInManager;
		}
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if(ModelState.IsValid) //server side validation
            {
                var User = new ApplicationUser()
                {
                    UserName = model.Email.Split('@')[0],
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
					IsAgree = model.IsAgree,
                };
                var Result =await _userManager.CreateAsync(User,model.Password);
                if (Result.Succeeded)
                        return RedirectToAction(nameof(Login));
                else
                    foreach(var err in Result.Errors)
                        ModelState.AddModelError(string.Empty, err.Description);
            }
            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if(ModelState.IsValid)
            {
                var User=await _userManager.FindByEmailAsync(model.Email);
                if(User is not null)
                {
                    var Flag =await _userManager.CheckPasswordAsync(User, model.Password);
                    if(Flag)
                    {
                        var Result = await _signInManager.PasswordSignInAsync(User,model.Password,model.RememberMe,false);
                        if (Result.Succeeded)
                            return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError(string.Empty, "Password Is InCorrect");
                }
                else
                    ModelState.AddModelError(string.Empty, "Email is not existed");
            }
            return View(model);
        } 
		public new async Task<IActionResult> SignOut()
		{
			await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
		}
		public IActionResult ForgetPassword()
        {
            return View();
        }
        public IActionResult CheckYourInbox()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User =await _userManager.FindByEmailAsync(model.Email);
                if(User is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(User);
                    var ResetPasswordLink= Url.Action("ResetPassword","Account",new {email =User.Email , Token = token},Request.Scheme);
                    var email = new Email()
                    {
                        Subject="Reset Password",
                        To=model.Email,
                        Body = ResetPasswordLink
                    };
                    EmailSettings.SendEmail(email);
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                else
                {
                    ModelState.AddModelError(string.Empty,"Email is not existed");
                }
            }
            else
            {
                return View(nameof(ForgetPassword), model);
            }
            return View();
        }   
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["Email"] = email;
            TempData["Token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                string email = TempData["Email"] as string;
                string token = TempData["Token"] as string;
                var User = await _userManager.FindByEmailAsync(email);
                if(User is not null)
                {
                   var Result= await _userManager.ResetPasswordAsync(User, token, model.NewPassword);
                    if(Result.Succeeded)
                        return RedirectToAction(nameof(Login));
                    else
                    {
                        foreach(var err in Result.Errors)
                            ModelState.AddModelError(string.Empty, err.Description);
                    }
                }
            }
            return View(model);
        }
        


    }
}

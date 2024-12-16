using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Demo.PL.Controllers
{
	[Authorize("Admin")]
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserController(UserManager<ApplicationUser> userManager , IMapper mapper)
		{
			_userManager = userManager;
            _mapper = mapper;
        }
		public async Task<IActionResult> Index(string searchString)
		{

			if (!searchString.IsNullOrEmpty())
			{
				var User = await _userManager.FindByEmailAsync(searchString);
				var MappedUser = new UserViewModel()
				{

					Id = User.Id,
					Fname = User.FirstName,
					Lname = User.LastName,
					Email = User.Email,
					PhoneNumber = User.PhoneNumber,
					Roles = _userManager.GetRolesAsync(User).Result
				};
				return View(User);
			}
			else
			{
				var Users = await _userManager.Users.Select(U => new UserViewModel()
				{
					Id = U.Id,
					Fname = U.FirstName,
					Lname = U.LastName,
					Email = U.Email,
					PhoneNumber = U.PhoneNumber,
					Roles = _userManager.GetRolesAsync(U).Result
				}).ToListAsync();
				return View(Users);
			}
		}
		public async Task<IActionResult> Details(string Id , string ViewName)
		{
			if(Id is null)
				return BadRequest();
			var User=await _userManager.FindByIdAsync(Id);
			
			if(User == null) 
				return NotFound();
			var MappedUser = _mapper.Map<UserViewModel>(User);
			return View(ViewName,MappedUser);	
			
		}
		public async Task<IActionResult> Edit(string Id)
		{
			return await Details(Id,"Edit");
		}
		[HttpPost]
		public async Task<IActionResult> Edit(UserViewModel model , [FromRoute] string Id)
		{
			if(model.Id  != Id)
				return BadRequest();
			if (ModelState.IsValid)
			{
				try
				{
					var User = await _userManager.FindByIdAsync(Id);
					User.PhoneNumber = model.PhoneNumber;
					User.LastName = model.Lname;
					User.FirstName = model.Fname;
					await _userManager.UpdateAsync(User);
					return RedirectToAction(nameof(Index));	
				}catch (Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}

			}
			return View(model);
			
			
		}
		public async Task<IActionResult> Delete(string Id)
		{
			return await Details(Id, "Delete");
		}
        [HttpPost]
        public async Task<IActionResult> ConfirmDelete([FromRoute] string Id)
		{
			try
			{
				var User = await _userManager.FindByIdAsync(Id);
				await _userManager.DeleteAsync(User);
				return RedirectToAction(nameof(Index));
			}catch (Exception ex)
			{
				ModelState.AddModelError(string.Empty, ex.Message);
				return RedirectToAction("Error", "Home");
			}			
			
		}
	}
}

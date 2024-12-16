using AutoMapper;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Demo.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager , IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            
            if(!searchString.IsNullOrEmpty())
            {
                var SearchedRole =await _roleManager.FindByNameAsync(searchString);
                var MappedRole=_mapper.Map<RoleViewModel>(SearchedRole);
                return View(new List<RoleViewModel>() { MappedRole});
            }
            else
            {
                var Roles = await _roleManager.Roles.ToListAsync();
                var MappedRoles = _mapper.Map<IEnumerable<RoleViewModel>>(Roles);
                return View(MappedRoles);
            }

        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Role = _mapper.Map<IdentityRole>(model);
                var Result = await _roleManager.CreateAsync(Role);
                if (Result.Succeeded)
                {
                    TempData["Message"] = "New Role Is Created";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach(var Error in Result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, Error.Description);
                    }
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Details(string Id, string ViewName)
        {
            if (Id is null)
                return BadRequest();
            var Role = await _roleManager.FindByIdAsync(Id);

            if (Role == null)
                return NotFound();
            var MappedRole = _mapper.Map<RoleViewModel>(Role);
            return View(ViewName, MappedRole);

        }

        public async Task<IActionResult> Edit(string Id)
        {
            return await Details(Id, "Edit");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(RoleViewModel model, [FromRoute] string Id)
        {
            if (model.Id != Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var Role = await _roleManager.FindByIdAsync(Id);
                    Role.Name = model.RoleName;
                    await _roleManager.UpdateAsync(Role);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
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
        public async Task<IActionResult> ConfirmDelete(string Id)
        {
            try
            {
                var Role = await _roleManager.FindByIdAsync(Id);
                    await _roleManager.DeleteAsync(Role);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Error", "Home");
            }

        }
    }
}

using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult>Index()
        {
            var departments= await _unitOfWork.DepartmentRepository.GetAllAsync();    
            return View(departments);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Create(Department department)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.DepartmentRepository.AddAsync(department);
                int Result =await _unitOfWork.CompleteAsync();
                if (Result > 0)
                {
                    TempData["Message"] = "Department is added";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }
        public async Task<IActionResult>Details(int? id,string ViewName)
        {
            if(id is null)
                return BadRequest();//return status code 400
            var department =await _unitOfWork.DepartmentRepository.GetByidAsync(id.Value);
            if (department is null)
                return NotFound();
            return View(department);
        } 
        public async Task<IActionResult> Edit(int? id)
        {
            return await Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Department department, [FromRoute] int? id)
        {
            if (department.Id!=id)
                return BadRequest();
            if(ModelState.IsValid)
            {
                try
                {
                    _unitOfWork.DepartmentRepository.Update(department);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty,ex.Message) ;
                    return View(department);    
                }
            }
            return View(department);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(Department department, [FromRoute] int? id)
        {
            if (department.Id != id)
                return BadRequest();
           
                try
                {
                    _unitOfWork.DepartmentRepository.Delete(department);
                    await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(department);
                }
        }


    }
}

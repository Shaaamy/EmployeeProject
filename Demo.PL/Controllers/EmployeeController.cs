using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Demo.PL.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchString)
        {
            IEnumerable<Employee> employees;
            employees = await _unitOfWork.EmployeeRepository.GetAllAsync();
            
            if (!searchString.IsNullOrEmpty())
            {
                employees = _unitOfWork.EmployeeRepository.GetEmployeesByName(searchString);
            }
            var MappedEmployee = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);

            return View(MappedEmployee);






            ////1. ViewData => KeyValuePair[Dicionary Object]
            //// Transfer Data from Controller [Action] to its View
            //// .NET Framework 3.5
            //ViewData["Message"] = "Hello From ViewData";

            ////2. ViewBag => Dynamic Property [Based On Dynamic Keyword]
            //// Transfer Data from Controller [Action] to its View
            //// .NET Framework 4.0
            //ViewBag.Message= "Hello From ViewBag";
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(EmployeeViewModel employeeVM)
        {
            
            if(ModelState.IsValid)
            {
                employeeVM.ImageName= DocumentSettings.UploadFile(employeeVM.Image, "Images");
                var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                await _unitOfWork.EmployeeRepository.AddAsync(MappedEmployee);
                int Result = await _unitOfWork.CompleteAsync();
                if (Result > 0)
                    TempData["Message"] = "New Employee Added";
                return RedirectToAction(nameof(Index));
            }
            return View(employeeVM);
        }
        public async Task<IActionResult> Details(int? id,string ViewName)
        {
            if (id is null)
                return BadRequest();
            var employee = await _unitOfWork.EmployeeRepository.GetByidAsync(id.Value);
            if(employee is null)
                return NotFound();
            var MappedEmployee= _mapper.Map<Employee,EmployeeViewModel>(employee);
            return View(MappedEmployee);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Departments = await _unitOfWork.DepartmentRepository.GetAllAsync();
            return await Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeViewModel employeeVM, [FromRoute] int? id)
        {
            if(employeeVM.Id != id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    if(employeeVM.Image is not null) 
                    { 
                        employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");
                    }
                    var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                    _unitOfWork.EmployeeRepository.Update(MappedEmployee);
                    await _unitOfWork.CompleteAsync();
                    return RedirectToAction(nameof(Index));
                }catch(Exception ex)
                {
                    ModelState.AddModelError(string.Empty,ex.Message);
                    return View(employeeVM);
                }
            }
            return View(employeeVM);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(EmployeeViewModel employeeVM, [FromRoute] int? id)
        {
            if (employeeVM.Id != id)
                return BadRequest();
                try
                {
                var MappedEmployee = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);
                _unitOfWork.EmployeeRepository.Delete(MappedEmployee);
                var Result=await _unitOfWork.CompleteAsync();
                    if (Result > 0 && employeeVM.ImageName is not null)
                    {
                        DocumentSettings.DeleteFile(employeeVM.ImageName, "Images");
                    }
                return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(employeeVM);
                }
        }

    }
}

using AutoMapper.Execution;
using Company.Models.Models;
using Company.Services.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Company.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployeeService _employeeService { get; set; }
        private IDepartmentService _departmentService { get; set; }
        public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService)
        {
            _employeeService=employeeService;
            _departmentService=departmentService;
        }

        private async Task<List<Department>> GetDepartmentList()
        {
            return (await _departmentService.GetAllAsync()).ToList();
        }

        private async Task<List<Employee>> GetEmployeeList()
        {
            return (await _employeeService.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, 
            int?pageNumber, int pageSize, int currentPageSize, int? currentDepartmentId)
        {
            string exportedEmployeeTable = await _employeeService.exportEmployeeTable();

            ViewData["CurrentSort"] = sortOrder;
            ViewData["EmployeeNoParam"] = string.IsNullOrEmpty(sortOrder) ? "numDesc" : "";
            ViewData["NameParam"] = sortOrder == "name" ? "nameDesc" : "name";
            ViewData["SalaryParam"] = sortOrder == "salary" ? "salaryDesc" : "salary";
            ViewData["DepartmentNoParam"] = sortOrder == "deparNo" ? "deparNoDesc" : "deparNo";
            ViewData["lastModifyDateParam"] = sortOrder == "date" ? "dateDesc" : "date";
            
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            pageSize = currentPageSize == 0 ? pageSize = currentPageSize = 3 : currentPageSize;
            ViewData["CurrentPageSize"] = pageSize;


            // detailed department display
            if(currentDepartmentId != null)
            {
                Department selectedDepartment = await _departmentService.GetAsync((int)currentDepartmentId);
                TempData["selectedDepartmentId"] = selectedDepartment.departmentNo;
                TempData["selectedDepartmentName"] = selectedDepartment.departmentName;
                TempData["selectedDepartmentLocation"] = selectedDepartment.departmentLocation;
            }
            

            return (View(await _employeeService.GetAllFilters(sortOrder, searchString, pageNumber, pageSize, currentPageSize)));
        }

        // GET Create
        public async Task<IActionResult> Create()
        {
            ViewData["Departments"] = (await GetDepartmentList()).Select(obj => new SelectListItem
            {
                Text="("+obj.departmentNo+") "+obj.departmentName+", "+obj.departmentLocation,
                Value=obj.departmentNo.ToString()
            }).ToList();
            return await Task.FromResult(View());
        }

        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee obj)
        {
            await _employeeService.CreateAsync(obj);
            return RedirectToAction("Index");
        }

        // GET Delete
        public async Task<IActionResult>Delete(int id)
        {
            var obj = await _employeeService.GetAsync(id);
            ViewData["DepartmentName"] = _departmentService.GetAsync(obj.EdepartmentNo).Result.departmentName;
            if (obj != null) return View(obj);
            return NotFound();
        }

        // POST Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Delete(Employee obj)
        {
            if(await _employeeService.DeleteAsync(obj.employeeNo)) return RedirectToAction("Index");
            return View(obj);
        }

        // GET Update
        public async Task<IActionResult>Update(int id)
        {
            
            ViewData["Departments"] = (await GetDepartmentList()).Select(obj => new SelectListItem
            {
                Text="("+obj.departmentNo+") "+obj.departmentName+", "+obj.departmentLocation,
                Value=obj.departmentNo.ToString()
            }).ToList();
            var obj = await _employeeService.GetAsync(id);
            if (obj != null) return View(obj);
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Update(Employee obj)
        {
            await _employeeService.UpdateAsync(obj.employeeNo, obj);
            return await Task.FromResult(RedirectToAction("Index"));
        }
    

        // GET - salary update by perc
        public async Task<IActionResult> SalaryUpdate(int id)
        {
            var obj = await _employeeService.GetAsync(id);
            if (obj != null) return View(obj);
            return NotFound();
        }

        // POST - salary update by perc
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalaryUpdate(Employee obj, int perc)
        {
            if (obj != null)
            {
                await _employeeService.ChangeSalary(obj.employeeNo, perc);
                return await Task.FromResult(RedirectToAction("SalaryUpdate", obj.employeeNo));
            }
            return await Task.FromResult(RedirectToAction("Index"));   
        }

    }
}

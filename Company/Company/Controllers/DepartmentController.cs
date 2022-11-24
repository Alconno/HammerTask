using Company.Models.Models;
using Company.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.WebPages.Html;
using Company.Models;
using Company.Models.Models.Filters;
using System.Diagnostics;

namespace Company.Controllers
{
    public class DepartmentController : Controller
    { 
        private IDepartmentService _departmentService { get; set; }
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService=departmentService;
        }
    
        private async Task<List<Department>> getDepartmentList()
        {
            return (await _departmentService.GetAllAsync()).ToList();
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber, int pageSize, int currentPageSize)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NumberSortParam"] = string.IsNullOrEmpty(sortOrder) ? "numDesc" : "";
            ViewData["NameSortParam"] = sortOrder == "name" ? "nameDesc" : "name";
            
            if(searchString != null)
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

            return (View(await _departmentService.GetAllFilters(sortOrder, searchString, pageNumber, pageSize, currentPageSize)));
        }

        // GET-Create
        public async Task<IActionResult> Create()
        {
            return await Task.FromResult(View());
        }

        // POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department obj)
        {
            var newObj = await _departmentService.CreateAsync(obj);
            return RedirectToAction("Index");
        }

        // GET-Delete
        public async Task<IActionResult> Delete(int id)
        {
            var obj = await _departmentService.GetAsync(id);
            if (obj != null) return await Task.FromResult(View(obj));
            return NotFound();
        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Department obj)
        {
            if (await _departmentService.DeleteAsync(obj.departmentNo)) return await Task.FromResult(RedirectToAction("Index"));
            return await Task.FromResult(View(obj));
        }

        // GET-Update
        public async Task<IActionResult> Update(int id)
        {
            var obj = await _departmentService.GetAsync(id);
            if(obj == null) return NotFound();
            return View(obj);
        }

        // POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Department obj)
        {   
            await _departmentService.UpdateAsync(obj.departmentNo, obj);   
            return await Task.FromResult(RedirectToAction("Index"));
        }
    }
}

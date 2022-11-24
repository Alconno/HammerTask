using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Models;
using Company;
using Company.Services.Data;
using Company.Models.Models;
using Company.Models.Models.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Company.Services.Services
{
    public class DepartmentService : IDepartmentService
    {
        private AppDbContext _db;
        public DepartmentService(AppDbContext db)
        {
            _db=db; 
        }

        public async Task<Department> CreateAsync(Department entity)
        {
            if(entity.departmentName.Length <= 20 && entity.departmentLocation.Length <= 20)
            {
                _db.Departments.Add(entity);
                _db.SaveChanges();
                return await Task.FromResult(entity);
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var obj = _db.Departments.Find(id);
            if(obj != null)
            {
                _db.Departments.Remove(obj);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<IQueryable<Department>> GetAllAsync()
        {
            return await Task.FromResult(_db.Departments);
        }

        public async Task<Department> GetAsync(int id)
        {
            return await Task.FromResult(_db.Departments.Find(id));
        }

        public async Task<Department> UpdateAsync(int No, Department entity)
        {
            if (entity != null)
            {   
                entity.departmentNo = No;
                _db.Departments.Update(entity);
                _db.SaveChanges();
                return await Task.FromResult(entity);
            }
            return null;
        }

        public async Task<Pagination<Department>> GetAllFilters(string sortOrder, string searchString, int? pageNumber, int pageSize, int currentPageSize)
        {
            Sort<Department> sort = new Sort<Department>();
            Filter filter = new Filter();
            var obj = sort.sortDepartmentList(filter.filterDepartmentList(GetAllAsync().Result, searchString).Result, sortOrder);

            return await Pagination<Department>.CreateAsync(
                                obj.AsNoTracking(),
                                pageNumber > Math.Ceiling(Convert.ToDouble(obj.Count()) / Convert.ToDouble(pageSize)) ? 1 : pageNumber ?? 1,
                                pageSize);
        }
    }
}

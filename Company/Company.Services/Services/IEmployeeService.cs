using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Models.Models;
using Company.Models.Models.Filters;

namespace Company.Services.Services
{
    public interface IEmployeeService : IServiceBase<Employee>
    {
        Task<Employee> CreateAsync(Employee entity);
        Task<bool> DeleteAsync(int No);
        Task<IQueryable<Employee>> GetAllAsync();
        Task<Pagination<Employee>> GetAllFilters(string sortOrder, string searchString, int? pageNumber, int pageSize, int currentPageSize);
        Task<Employee> GetAsync(int No);
        Task<Employee> UpdateAsync(int No, Employee entity);
        Task<Employee> ChangeSalary(int No, int perc);
        Task<string> exportEmployeeTable();
        Task<IEnumerable<(Employee Employee, Department Department)>> GetCombinedObjectList();
        Task DoLinqThings();

    }
}

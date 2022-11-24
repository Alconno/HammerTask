using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Company.Models.Models;
using Company.Models.Models.Filters;

namespace Company.Services.Services
{
    public interface IDepartmentService : IServiceBase<Department>
    {
        Task<Department> CreateAsync(Department entity);
        Task<bool> DeleteAsync(int No);
        Task<IQueryable<Department>> GetAllAsync();
        Task<Pagination<Department>> GetAllFilters(string sortOrder, string searchString, int? pageNumber, int pageSize, int currentPageSize);
        Task<Department> GetAsync(int No);
        Task<Department> UpdateAsync(int No, Department entity);
    }
}

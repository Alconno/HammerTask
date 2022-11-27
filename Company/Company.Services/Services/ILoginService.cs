using Company.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Services
{
    public interface ILoginService : IServiceBase<Login>
    {
        Task<Login> CreateAsync(Login entity);
        Task<bool> DeleteAsync(int No);
        Task<IQueryable<Login>> GetAllAsync();
        //Task<Pagination<VehicleMake>> GetAllFilters(string sortOrder, string searchString, int? pageNumber, int pageSize, int currentPageSize);
        Task<Login> GetAsync(int No);
        Task<Login> GetAsync(string name);
        Task<Login> GetAsync(string name, string pass);
        Task<Login> UpdateAsync(int No, Login entity);
    }
}
using System.Linq;
using System.Threading.Tasks;

namespace Company.Models.Models.Filters
{
    public interface IFilter
    {
        Task<IQueryable<Department>> filterDepartmentList(IQueryable<Department> list, string currentFilter);
        Task<IQueryable<Employee>> filterEmployeeList(IQueryable<Employee> list, string currentFilter);
        Task<IQueryable<Login>> filterLoginList(IQueryable<Login> list, string currentFilter);
    }
}
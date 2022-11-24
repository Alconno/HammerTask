using System.Linq;

namespace Company.Models.Models.Filters
{
    public interface ISort
    {
        IQueryable<Department> sortDepartmentList(IQueryable<Department> list, string sortOrder);
        IQueryable<Employee> sortEmployeeList(IQueryable<Employee> list, string sortOrder);
        IQueryable<Login> sortLoginList(IQueryable<Login> list, string sortOrder);
    }
}
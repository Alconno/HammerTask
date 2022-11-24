using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models.Filters
{
    public class Filter : IFilter
    {
        public async Task<IQueryable<Login>> filterLoginList(IQueryable<Login> list, string currentFilter)
        {
            if (currentFilter != null)
            {
                list = await Task.FromResult((IQueryable<Login>)list.Where(s => s.loginUserName.Contains(currentFilter)));
            }
            return list;
        }

        public async Task<IQueryable<Department>> filterDepartmentList(IQueryable<Department> list, string currentFilter)
        {
            if (currentFilter != null)
            {
                list = await Task.FromResult((IQueryable<Department>)list.Where(s => s.departmentName.Contains(currentFilter)
                                                                                || s.departmentLocation.Contains(currentFilter)));
            }
            return list;
        }

        public async Task<IQueryable<Employee>> filterEmployeeList(IQueryable<Employee> list, string currentFilter)
        {
            if (currentFilter != null)
            {
                list = await Task.FromResult((IQueryable<Employee>)list.Where(s => s.employeeName.Contains(currentFilter)
                                                                                || s.Salary.ToString().Contains(currentFilter)
                                                                                || s.EdepartmentNo.ToString().Contains(currentFilter)
                                                                                || s.department.departmentName.Contains(currentFilter)
                                                                                || s.department.departmentLocation.Contains(currentFilter)
                                                                                || s.lastModifyDate.ToString().Contains(currentFilter)));
            }
            return list;
        }
    }
}

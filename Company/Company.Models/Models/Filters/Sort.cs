using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models.Filters
{
    public class Sort<T> : ISort
    {
        public IQueryable<Login> sortLoginList(IQueryable<Login> list, string sortOrder)
        {
            switch (sortOrder)
            {
                case "nameDesc":
                    list = list.OrderByDescending(s => s.loginUserName);
                    break;
                case "numDesc":
                    list = list.OrderByDescending(s => s.loginNo);
                    break;
                default:
                    list = list.OrderBy(s => s.loginNo);
                    break;
            }
            return list;
        }

        public IQueryable<Department> sortDepartmentList(IQueryable<Department> list, string sortOrder)
        {
            switch (sortOrder)
            {
                case "nameDesc":
                    list = list.OrderByDescending(s => s.departmentName);
                    break;
                case "name":
                    list = list.OrderBy(s => s.departmentName);
                    break;
                case "locationDesc":
                    list = list.OrderByDescending(s => s.departmentLocation);
                    break;
                case "location":
                    list = list.OrderBy(s => s.departmentLocation);
                    break;
                default:
                    list = list.OrderBy(s => s.departmentNo);
                    break;
            }
            return list;
        }

        public IQueryable<Employee> sortEmployeeList(IQueryable<Employee> list, string sortOrder)
        {
            switch (sortOrder)
            {
                case "nameDesc":
                    list = list.OrderByDescending(s => s.employeeName);
                    break;
                case "name":
                    list = list.OrderBy(s => s.employeeName);
                    break;
                case "salary":
                    list = list.OrderBy(s => s.Salary);
                    break;
                case "salaryDesc":
                    list = list.OrderByDescending(s => s.Salary);
                    break;
                case "deparNo":
                    list = list.OrderBy(s => s.EdepartmentNo);
                    break;
                case "deparNoDesc":
                    list = list.OrderByDescending(s => s.EdepartmentNo);
                    break;
                case "date":
                    list = list.OrderBy(s => s.lastModifyDate);
                    break;
                case "dateDesc":
                    list = list.OrderByDescending(s => s.lastModifyDate);
                    break;
                case "numDesc":
                    list = list.OrderByDescending(s => s.employeeNo);
                    break;
                default:
                    list = list.OrderBy(s => s.employeeNo);
                    break;
            }
            return list;



            /*
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EmployeeNoParam"] = string.IsNullOrEmpty(sortOrder) ? "numDesc" : "";
            ViewData["NameParam"] = sortOrder == "name" ? "nameDesc" : "name";
            ViewData["SalaryParam"] = sortOrder == "salary" ? "salaryDesc" : "salary";
            ViewData["DepartmentNoParam"] = sortOrder == "deparNo" ? "deparNoDesc" : "deparNo";
            ViewData["lastModifyDateParam"] = sortOrder == "date" ? "dateDesc" : "date";
            */
        }
    }
}

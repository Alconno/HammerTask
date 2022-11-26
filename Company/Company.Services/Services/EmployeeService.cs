using AutoMapper;
using AutoMapper.Mappers;
using Company.Models.Models;
using Company.Models.Models.Filters;
using Company.Services.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Company.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        AppDbContext _db;
    
        public EmployeeService(AppDbContext db)
        {
            _db=db;
        }
    
        public async Task<Employee> CreateAsync(Employee entity)
        {
            var deparObj = await Task.FromResult(_db.Departments.Find(entity.EdepartmentNo));
            if (entity.employeeName.Length <= 50 && deparObj != null && entity.Salary > 0)
            {
                // make name and surname start with capital letters
                entity.employeeName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(entity.employeeName.ToLower());

                // assign necessary values and update database
                entity.department = deparObj; // assign deparment to employee
                entity.EdepartmentNo = deparObj.departmentNo;
                entity.lastModifyDate = DateTime.Now;
                _db.Employees.Add(entity);
                _db.SaveChanges();
                return await Task.FromResult(entity);
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int No)
        {
            var obj = await Task.FromResult(_db.Employees.Find(No));
            if(obj != null)
            {
                _db.Employees.Remove(obj);
                _db.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<IQueryable<Employee>> GetAllAsync()
        {
            return await Task.FromResult(_db.Employees);
        }

        public async Task<Employee> GetAsync(int No)
        {
            return await Task.FromResult(_db.Employees.Find(No));
        }

        public async Task<Employee> UpdateAsync(int No, Employee entity)
        {
            if (entity != null)
            {
                entity.lastModifyDate = DateTime.Now;
                entity.employeeNo = No;
                entity.department = _db.Departments.Find(entity.EdepartmentNo);
                _db.Employees.Update(entity);
                _db.SaveChanges();
                return await Task.FromResult(entity);
            }
            return null;
        }

        public async Task<Pagination<Employee>> GetAllFilters(string sortOrder, string searchString, int? pageNumber, int pageSize, int currentPageSize)
        {
            Sort<Employee> sort = new Sort<Employee>();
            Filter filter = new Filter();
            var obj = sort.sortEmployeeList(filter.filterEmployeeList(GetAllAsync().Result, searchString).Result, sortOrder);

            return await Pagination<Employee>.CreateAsync(
                                obj.AsNoTracking(),
                                pageNumber > Math.Ceiling(Convert.ToDouble(obj.Count()) / Convert.ToDouble(pageSize)) ? 1 : pageNumber ?? 1,
                                pageSize);
        }

        // This method changes salary by given percentage, negative percentage = salary decrease, positive percentage = salary increase
        public async Task<Employee> ChangeSalary(int No, int perc)
        {
            var obj = _db.Employees.Find(No);
            if (obj != null)
            {
                double salary = obj.Salary;
                if (perc > 0)
                {
                    salary *= (Convert.ToDouble(perc)/Convert.ToDouble(100)+1);
                    Debug.WriteLine(salary);
                }
                else if(perc > 0 && perc < 99)
                {
                    salary -= Math.Abs(Convert.ToDouble(perc))/Convert.ToDouble(100)*salary;
                }
                if((int)salary != 0) obj.Salary = (int)salary;
                _db.Employees.Update(obj);
                _db.SaveChanges();
                return await Task.FromResult(obj);
            }
            return null;
        }

        // This method returns a string containing all the Employee data as a table display and prints it into the Debug console
        public async Task<string> exportEmployeeTable()
        {
            string numberFormat = "### ###.00";
            IEnumerable<string> columnNames = new string[]{
                "employeeNo",
                "employeeName",
                "Salary",
                "departmentNo"
            };

            List<Employee> employees = await _db.Employees.ToListAsync();
            if (!employees.Any()) return "";

            List<int> lengthOfElementOffsets = new List<int>{
                Math.Max(columnNames.ElementAt(0).Length, employees.Max(x => x.employeeNo.ToString().Length)),
                Math.Max(columnNames.ElementAt(1).Length, employees.Max(x => x.employeeName.Length)),
                Math.Max(columnNames.ElementAt(2).Length, employees.Max(x => x.Salary.ToString(numberFormat).Length)),
                Math.Max(columnNames.ElementAt(3).Length, employees.Max(x => x.EdepartmentNo.ToString().Length))
            };

            int defaultUsedSpace = lengthOfElementOffsets.Sum() + 5; // Summing the element lengths + adding offset (+ signs)

            int maximumRowLength = lengthOfElementOffsets.Max();

            Debug.WriteLine(string.Join(" ", lengthOfElementOffsets));
            // Generate the empty row string. Example: (+------------------+-------------------+-------------------+-------------------+)
            string emptyRow = "+" + string.Join("+", lengthOfElementOffsets.Select(x => new string('-', x))) + "+";

            // Render the table, starting with the empty row
            string table = $"{emptyRow}\n";

            // Output the table
            List<IEnumerable<string>> rows = new List<IEnumerable<string>> { columnNames };
            rows.AddRange(employees.Select(x => new string[] {
                x.employeeNo.ToString(),
                x.employeeName,
                x.Salary.ToString(numberFormat),
                x.EdepartmentNo.ToString()
            }));

            foreach (var row in rows)
            {
                foreach (var kvp in row.Select((element, idx) => (element, idx)))
                {
                    table += "|" + kvp.element + new string(' ', lengthOfElementOffsets.ElementAt(kvp.idx) - kvp.element.Length);
                }
                table += $"|\n{emptyRow}\n";
            }

            Debug.WriteLine(table);

            return await Task.FromResult(table);
        }

        // This method takes all employees and departments and joins them into a single object pair (every employee has their department info)
        public async Task<IEnumerable<(Employee Employee, Department Department)>> GetCombinedObjectList()
        {
            List<Employee> employees = await _db.Employees.ToListAsync();
            List<Department> departments = await _db.Departments.ToListAsync();

            return employees.Join(departments,
                employee => employee.EdepartmentNo,
                department => department.departmentNo,
                (employee, department) => (employee, department));
        }

        public async Task DoLinqThings()
        {
            var data = await GetCombinedObjectList();

            // 1 | Get average salary for 'Development' employees situated in any location besides 'London'
            double averageDeveloperSalaryNotInLondon =  data.Where(x => x.Department.departmentLocation != "London" && x.Department.departmentName == "Development")
                                                            .Average(x => x.Employee.Salary);

            Debug.WriteLine($"What is the average salary for a Development employee in any location except London?\n" +
                            $"= {averageDeveloperSalaryNotInLondon}");

            // 2 | Output locations which have more than 1 employee and how many employees are there
            var employeesPerLocation =  data.GroupBy(x => x.Department.departmentLocation)
                                            .Select(x => (
                                                location: x.Key,
                                                employeeCount: x.Count(y => y.Department.departmentName == "Development")))
                                            .Where(x => x.employeeCount > 1);

            Debug.WriteLine($"\nWhich locations have more than one employee? What are those locations and how many employees are in each?");
            foreach (var locationCount in employeesPerLocation)
            {
                Debug.WriteLine($"= {locationCount.location} has {locationCount.employeeCount} employees");
            }


            // 3 | How many development employees are in each location
            var devEmployeesPerLocation = data.GroupBy(x => x.Department.departmentLocation)
                                            .Select(x => (
                                                location: x.Key,
                                                employeeCount: x.Count(y => y.Department.departmentName == "Development")
                                            ));

            Debug.WriteLine($"\nHow many Development employees are in each location?");
            foreach (var locationCount in devEmployeesPerLocation)
            {
                Debug.WriteLine($"= {locationCount.location} has {locationCount.employeeCount} Development employees");
            }

            // 4 | Get the second highest salary from all employees
            int secondHighestSalaryResult = data.Select(x => x.Employee)
                                            .GroupBy(x => x.Salary) // if you want the second highest salary keep this line, if you want salary of the second highest paid person comment this line out
                                            .OrderByDescending(x => x.Key) // order by x.Key if grouping, else order by x.Salary
                                            .Skip(1)
                                            .FirstOrDefault()?.First().Salary ?? -1;
            Debug.WriteLine($"\nWhat is the second highest salary?\n" +
                            $"= {secondHighestSalaryResult}");

            
        }

    }
}

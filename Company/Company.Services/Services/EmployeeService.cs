using AutoMapper;
using AutoMapper.Mappers;
using Company.Models.Models;
using Company.Models.Models.Filters;
using Company.Services.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> exportEmployeeTable()
        {
            int rowLength = 80;
            string table = "", Emptyrow = "";
            string[] elements = { "employeeNo", "employeeName", "Salary", "departmentNo" };
            var employees = await _db.Employees.ToListAsync(); // get all employees
            if (!employees.Any()) return table;

            int[] l = { Math.Max(elements[0].Length, employees.Select(a => a.employeeNo.ToString()).ToList().OrderByDescending(s => s.Length).First().Length), // employeeNoLen
                        Math.Max(elements[1].Length, employees.Select(a => a.employeeName).ToList().OrderByDescending(s => s.Length).First().Length), // employeeNameLen
                        Math.Max(elements[2].Length, employees.Select(a => (a.Salary > 1000 ? 1 : 0) + a.Salary.ToString()).ToList().OrderByDescending(s => s.Length).First().Length+2), // salaryLen
                        Math.Max(elements[3].Length, employees.Select(a => a.EdepartmentNo.ToString()).ToList().OrderByDescending(s => s.Length).First().Length)}; // departmentNoLen
            int defaultUsedSpace = l.Aggregate(0, (a, b) => a+=b)+5;

            while(defaultUsedSpace < rowLength)
            {
                if (l[0] < l[1] && l[0] < l[2] && l[0] < l[3]) l[0]++;
                else if(l[1] < l[2] && l[1] < l[2]) l[1]++;
                else if(l[2] < l[3]) l[2]++;
                else l[3]++;
                defaultUsedSpace++;
            }
            Debug.WriteLine(l[0].ToString()+", "+l[1].ToString()+", "+l[2].ToString()+", "+l[3].ToString());

            // generate looks of empty row (+------------------+-------------------+-------------------+-------------------+)
            for (int i = 0; i < elements.Count(); i++)
            {
                Emptyrow += '+';
                for (int j = 0; j < l[i]; j++) Emptyrow += '-';
            }
            Emptyrow += '+';

            // generate top row of element names
            table += Emptyrow + "\n" + '|' + elements[0];
            for (int i = 0; i < l[0]-elements[0].Length; i++) table += ' ';
            table += '|' + elements[1];
            for (int i = 0; i < l[1]-elements[1].Length; i++) table += ' ';
            table += '|' + elements[2];
            for (int i = 0; i < l[2]-elements[2].Length; i++) table += ' ';
            table += '|' + elements[3];
            for (int i = 0; i < l[3]-elements[3].Length; i++) table += ' ';
            table += "|\n" + Emptyrow + '\n';

            // generate rows of data under element names
            for (int i = 0; i < employees.Count(); i++)
            {
                var f = new NumberFormatInfo { NumberGroupSeparator=" " };
                string[] rowData = { employees[i].employeeNo.ToString(), employees[i].employeeName, employees[i].Salary.ToString("n", f), employees[i].EdepartmentNo.ToString() };
                table += '|'+rowData[0];
                for (int j = 0; j < l[0]-rowData[0].Length; j++) table += ' ';
                table += '|'+rowData[1];
                for (int j = 0; j < l[1]-rowData[1].Length; j++) table += ' ';
                table += '|'+rowData[2];
                for (int j = 0; j < l[2]-rowData[2].Length; j++) table += ' ';
                table += '|'+rowData[3];
                for (int j = 0; j < l[3]-rowData[3].Length; j++) table += ' ';
                table +="|\n"+Emptyrow+'\n';
            }
            Debug.WriteLine(table);
            

            return await Task.FromResult(table);
        }
        
    }
}

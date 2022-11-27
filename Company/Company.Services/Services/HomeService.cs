using Company.Models.Models;
using Company.Services.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Services
{
    public class HomeService : IHomeService
    {
        AppDbContext _db;
        IDepartmentService _departmentService;
        ILoginService _loginService;
        IEmployeeService _employeeService;
        public HomeService(AppDbContext db, IDepartmentService departmentService, ILoginService loginService, IEmployeeService employeeService)
        {
            _db=db; 
            _departmentService=departmentService;
            _loginService=loginService; 
            _employeeService=employeeService;
        }

        public async Task addDefaultData()
        {
            if (_db.Logins.ToList().Count()==0 && _db.Departments.ToList().Count()==0 && _db.Employees.ToList().Count()==0)
            {
                DateTime time = DateTime.Now;
                
                Login[] users = {   new Login(){loginUserName="Bill", loginPassword=new Encrypt().Hash("ItsNotSoft")},
                                    new Login(){loginUserName="Jean", loginPassword=new Encrypt().Hash("trollsRule")} };
                
                Department[] departments ={ new Department(){departmentName="Development", departmentLocation="London"},
                                            new Department(){departmentName="Development", departmentLocation="Zurich" },
                                            new Department(){departmentName="Development", departmentLocation="Osijek" },
                                            new Department(){departmentName="Sales", departmentLocation="London" },
                                            new Department(){departmentName="Sales", departmentLocation="Zurich" },
                                            new Department(){departmentName="Sales", departmentLocation="Osijek" },
                                            new Department(){departmentName="Sales", departmentLocation="Basel" },
                                            new Department(){departmentName="Sales", departmentLocation="Lugano", }
                                            };
                
                Employee[] employees = {new Employee(){employeeName="Fred Davies",   Salary=50000, lastModifyDate=time, EdepartmentNo=4},
                                         new Employee(){employeeName="Bernard Katic", Salary=50000, lastModifyDate=time, EdepartmentNo=3},
                                         new Employee(){employeeName="Rich Davies",   Salary=30000, lastModifyDate=time, EdepartmentNo=5},
                                         new Employee(){employeeName="Eva Dobos",     Salary=30000, lastModifyDate=time, EdepartmentNo=6},
                                         new Employee(){employeeName="Mario Hunjadi", Salary=25000, lastModifyDate=time, EdepartmentNo=8},
                                         new Employee(){employeeName="Jean Michele",  Salary=25000, lastModifyDate=time, EdepartmentNo=7},
                                         new Employee(){employeeName="Bill Gates",    Salary=25000, lastModifyDate=time, EdepartmentNo=1},
                                         new Employee(){employeeName="Maja Janic",    Salary=30000, lastModifyDate=time, EdepartmentNo=3},
                                         new Employee(){employeeName="Igor Horvat",   Salary=35000, lastModifyDate=time, EdepartmentNo=3},
                                         };

                foreach (Login user in users) await _loginService.CreateAsync(user);
                foreach (Department department in departments) await _departmentService.CreateAsync(department);
                foreach (Employee employee in employees)
                {
                    employee.department = await _departmentService.GetAsync(employee.EdepartmentNo);
                    await _employeeService.CreateAsync(employee);
                }

                Debug.WriteLine("------------------------------------------Added default db data------------------------------------------");
            }
            
        }
    }
}

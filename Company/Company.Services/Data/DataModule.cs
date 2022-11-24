using Autofac;
using Company.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Data
{
    public class DataModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoginService>().As<ILoginService>();
            builder.RegisterType<DepartmentService>().As<IDepartmentService>();
            builder.RegisterType<EmployeeService>().As<IEmployeeService>();
        }
    }
}

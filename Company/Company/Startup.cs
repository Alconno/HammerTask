using Autofac;
using Autofac.Integration.Mvc;
using Company.Controllers;
using Company.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using Company.Models.Models;
using Company.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Company.Services.Data;
using Company.Services.Services;
using Microsoft.IdentityModel.Protocols;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Company
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ContainerBuilder();




            // Register MVC Controllers
            builder.RegisterType<HomeController>().InstancePerRequest();
            builder.RegisterType<DepartmentController>().InstancePerRequest();
            builder.RegisterType<EmployeeController>().InstancePerRequest();
            builder.RegisterType<UserController>().InstancePerRequest();
            //builder.RegisterControllers(typeof(MvcApplication).Assembly);


            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(Department).Assembly);
            builder.RegisterModelBinders(typeof(Employee).Assembly);
            builder.RegisterModelBinders(typeof(Login).Assembly);
            builder.RegisterModelBinders(typeof(ErrorViewModel).Assembly);
            builder.RegisterModelBinderProvider();

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();
            builder.Register(c => Configuration).As<IConfiguration>().SingleInstance();

            builder.RegisterModule(new DataModule());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add stuff yes
            services.AddControllersWithViews();
            services.AddControllers();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession();

            // DB
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("Company.Services"))
            );

            // Authentication with Bearer and JWT configuration
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new
                    SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes
                    (Configuration["Jwt:Key"]))
                };
            });




            // DI
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<IEncrypt, Encrypt>();
            services.AddScoped<IHomeService, HomeService>();
            //services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<ITokenService, TokenService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSession();
            app.Use(async (context, next) =>
            {
                var token = context.Session.GetString("Token");
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Add("Authorization", "Bearer " + token);
                }
                await next();
            });

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
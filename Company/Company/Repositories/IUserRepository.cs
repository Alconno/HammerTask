using Company.Models.Models;
using Company.Services.Services;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Owin;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace Company.Repositories
{
    public interface IUserRepository
    {
        Login getUser(Login user);
    }
}

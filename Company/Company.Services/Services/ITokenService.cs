using Company.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Services
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, Login user);
        bool ValidateToken(string key, string issuer, string token);
    }
}

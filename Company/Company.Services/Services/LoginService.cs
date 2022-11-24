using Company.Models.Models;
using Company.Services.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Company.Services.Services
{
    public class LoginService : ILoginService
    {
        AppDbContext _db;
        public LoginService(AppDbContext db)
        {
            _db=db;
        }
    
        public async Task<Login> CreateAsync(Login entity)
        {
            if(entity.loginUserName.Length <= 20)
            {
                _db.Logins.Add(entity);
                _db.SaveChanges();
                return await Task.FromResult(entity);
            }
            return null;
        }

        public async Task<bool> DeleteAsync(int No)
        {
            var obj = await Task.FromResult(_db.Logins.Find(No));
            if(obj != null)
            {
                _db.Logins.Remove(obj);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
        
        public async Task<IQueryable<Login>> GetAllAsync()
        {
            return await Task.FromResult(_db.Logins);
        }

        public async Task<Login> UpdateAsync(int No, Login entity)
        {
            if (entity.loginUserName.Length <= 20 && entity.loginPassword.Length <= 20)
            {
                entity.loginNo = No;
                _db.Logins.Update(entity);
                _db.SaveChanges();
                return await Task.FromResult(entity);
            }
            return null;
        }

        public async Task<Login> GetAsync(string name, string pass)
        {
            // even tho there should be unique names, doing it this way for whatever safety
            string epass = new Encrypt().Hash(pass);
            var listWithMatchingUsernames = _db.Logins.Where(n => n.loginUserName == name).ToListAsync().Result;
            for (int i = 0; i < listWithMatchingUsernames.Count(); i++)
            {
                if (listWithMatchingUsernames.ElementAt(i).loginPassword == epass)
                {
                    return await Task.FromResult(listWithMatchingUsernames.ElementAt(i));
                }
            }
            return null;
        }

        public async Task<Login> GetAsync(string name)
        {
            var listWithMatchingUsernames = _db.Logins.Where(n => n.loginUserName == name).ToListAsync().Result;
            if(listWithMatchingUsernames.Count()>=1) return await Task.FromResult(listWithMatchingUsernames.ElementAt(0));
            return null;
        }

        public async Task<Login> GetAsync(int No)
        {
            var listWithMatchingNo = _db.Logins.Where(n => n.loginNo == No).ToListAsync().Result;
            if (listWithMatchingNo.Count()>=1) return await Task.FromResult(listWithMatchingNo.ElementAt(0));
            return null;
        }
    }
}

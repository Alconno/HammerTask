using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Services.Services
{
    public interface IServiceBase<T>
    {
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(int No, T entity);
        Task<bool> DeleteAsync(int No);
        Task<IQueryable<T>> GetAllAsync();
        Task<T> GetAsync(int No);
    }
}

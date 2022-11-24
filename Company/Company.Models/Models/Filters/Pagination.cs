using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Models.Models.Filters
{
    public class Pagination<T> : List<T>, IPagination
    {
        public int pageIndex { get; private set; }
        public int totalPages { get; private set; }

        public Pagination(List<T> items, int count, int pageIdx, int pageSize)
        {
            pageIndex = pageIdx;
            totalPages = (int)Math.Ceiling(count/(double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (pageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (pageIndex < totalPages);
            }
        }

        public static async Task<Pagination<T>> CreateAsync(IQueryable<T> source, int pageIdx, int pageSize)
        {
            var count = await Task.FromResult(source.Count());
            var items = await Task.FromResult(source.Skip((pageIdx-1)*pageSize).Take(pageSize).ToList());
            return await Task.FromResult(new Pagination<T>(items, count, pageIdx, pageSize));
        }


    }
}

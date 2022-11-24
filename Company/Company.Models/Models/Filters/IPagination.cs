using System.Linq;
using System.Threading.Tasks;

namespace Company.Models.Models.Filters
{
    public interface IPagination
    {
        bool HasNextPage { get; }
        bool HasPreviousPage { get; }
        int pageIndex { get; }
        int totalPages { get; }
    }
}
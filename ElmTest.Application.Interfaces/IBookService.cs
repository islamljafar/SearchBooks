using ElmTest.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmTest.Application.Interfaces
{
    public interface IBookService
    {
        
        Task<IEnumerable<Book>> Get(string search,int pageNumber);
        
    }
}

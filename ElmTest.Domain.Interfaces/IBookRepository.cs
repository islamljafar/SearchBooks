using ElmTest.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmTest.Domain.Interfaces
{
    public interface IBookRepository
    {
        public   Task<IEnumerable<Book>> Get(string search, int pageNumber);


    }
}

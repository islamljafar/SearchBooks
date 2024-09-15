using AutoMapper;
using ElmTest.Application.Interfaces;
using ElmTest.Domain.Entities;
using ElmTest.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElmTest.Application.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
        }

        public Task<IEnumerable<Book>> Get(string search ,int pageNumber)
        {
            return _bookRepository.Get(search,pageNumber);
        }
    }
}

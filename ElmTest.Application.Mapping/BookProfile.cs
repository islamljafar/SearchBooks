using AutoMapper;
using ElmTest.Domain.Entities;
using ElmTest.Application.Dto;
namespace ElmTest.Application.Mapping
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>().ReverseMap();
        }
    }
}
using AutoMapper;
using ElmTest.Application.Dto;
using ElmTest.Application.Interfaces;
using ElmTest.Domain.Entities;
using ElmTest.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ElmTest.Presentation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, IMapper mapper, ILogger<BooksController> logger) 
        {
            _bookService = bookService;
            _mapper = mapper;
            _logger = logger;

        }



      

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string search, [FromQuery] int pageNumber)
        {
            if (pageNumber < 1)
            {
                _logger.LogWarning("Invalid page number: {PageNumber}. Page number must be greater than 0.", pageNumber);
                return BadRequest(new ApiResponse
                {
                    StatusCode = 400,
                    Message = "Page number must be greater than 0."
                });
            }

            try
            {
                _logger.LogInformation("Fetching books with search term: {SearchTerm} and page number: {PageNumber}", search, pageNumber);
                var books = await _bookService.Get(search, pageNumber);
                if (books == null || !books.Any())
                {
                    _logger.LogWarning("No books found for search term: {SearchTerm} and page number: {PageNumber}", search, pageNumber);
                    return NotFound(new ApiResponse
                    {
                        StatusCode = 404,
                        Message = "No books found."
                    });
                }

                var bookDtos = _mapper.Map<IEnumerable<BookDto>>(books);

                var response = new ApiResponse<IEnumerable<BookDto>>
                {
                    StatusCode = 200,
                    Message = "Books retrieved successfully.",
                    Data = bookDtos,
                    Paging = new PagingInfo
                    {
                        PageNumber = pageNumber,
                        PageSize = 5  
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching books with search term: {SearchTerm} and page number: {PageNumber}", search, pageNumber);
                return StatusCode(500, new ApiResponse
                {
                    StatusCode = 500,
                    Message = "An error occurred while processing your request."
                });
            }
        }


    }
}
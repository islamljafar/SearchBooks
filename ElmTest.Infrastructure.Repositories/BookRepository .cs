using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using ElmTest.Domain.Entities;
using ElmTest.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace ElmTest.Infrastructure.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<BookRepository> _logger;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);
        private readonly IMemoryCache _cache;


        public BookRepository(IDbConnection dbConnection, ILogger<BookRepository> logger, IMapper mapper, IMemoryCache cache)
        {
            _dbConnection = dbConnection;
            _logger = logger;
            _mapper = mapper;
            _cache = cache;

        }


      

        public async Task<IEnumerable<Book>> Get(string search, int pageNumber)
        {
            string cacheKey = $"{search}-{pageNumber}";
            _logger.LogInformation("Starting Get method with search term: {SearchTerm} and page number: {PageNumber}", search, pageNumber);

            if (_cache.TryGetValue(cacheKey, out IEnumerable<Book> books))
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return books;
            }
            _logger.LogInformation("Cache miss for key: {CacheKey}", cacheKey);

            const string sqlQuery = @" IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BookDetails')
BEGIN
     CREATE TABLE BookDetails
    (
        BookId INT PRIMARY KEY,
        BookTitle NVARCHAR(255),
        Author NVARCHAR(255),
        BookDescription NVARCHAR(MAX),
        PublishDate DATE,
        CoverBase64 NVARCHAR(MAX)
    );

     CREATE NONCLUSTERED INDEX IX_BookDetails_BookTitle
    ON BookDetails (BookTitle);

     CREATE NONCLUSTERED INDEX IX_BookDetails_Author
    ON BookDetails (Author);

    
    INSERT INTO BookDetails (BookId, BookTitle, Author, BookDescription, PublishDate, CoverBase64)
	SELECT 
     
        b.BookId,
        JSON_VALUE(b.BookInfo, '$.BookTitle') AS BookTitle,
        JSON_VALUE(b.BookInfo, '$.Author') AS Author,
        JSON_VALUE(b.BookInfo, '$.BookDescription') AS BookDescription,
        CAST(JSON_VALUE(b.BookInfo, '$.PublishDate') AS DATE) AS PublishDate,
        j.CoverBase64
    FROM 

        Book b
    CROSS APPLY 
        OPENJSON(b.BookInfo)
        WITH (
            CoverBase64 NVARCHAR(MAX) '$.CoverBase64'
        ) j;
END
ELSE
BEGIN
    PRINT 'Table BookDetails already exists. Skipping creation and data insertion.';
END

 
  DECLARE @PageNumber INT = @PageNumberUser; 
DECLARE @PageSize INT = 5; 
DECLARE @SearchString NVARCHAR(255) = @SearchStringUser; 

SET STATISTICS TIME ON;
DBCC FREEPROCCACHE;

DECLARE @IsDate BIT = CASE 
    WHEN ISDATE(@SearchString) = 1 THEN 1
    ELSE 0
END;

SELECT 
    b.BookId,
    b.BookTitle,
    b.Author,
    b.BookDescription,
    b.PublishDate,
    b.CoverBase64
FROM 
    BookDetails b
WHERE 
    (@IsDate = 1 AND b.PublishDate LIKE '%' + @SearchString + '%') OR
    b.BookTitle LIKE '%' + @SearchString + '%' OR
    b.Author LIKE '%' + @SearchString + '%' OR
    b.BookDescription LIKE '%' + @SearchString + '%'
ORDER BY 
    b.BookId  
OFFSET @PageNumber * @PageSize ROWS 
FETCH NEXT @PageSize ROWS ONLY;

SET STATISTICS TIME OFF;

            ";
            try
            {
                books = await _dbConnection.QueryAsync<Book>(sqlQuery, new { SearchStringUser = search, PageNumberUser = pageNumber });
                _logger.LogInformation("Fetched {BookCount} books from database for search term: {SearchTerm}", books.Count(), search);

                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };

                _cache.Set(cacheKey, books, cacheEntryOptions);
                _logger.LogInformation("Cached books for key: {CacheKey}", cacheKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books from the database for search term: {SearchTerm}", search);
                throw;
            }

            return books;
        }
    }
}

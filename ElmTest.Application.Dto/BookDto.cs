namespace ElmTest.Application.Dto
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }
        public string? PublishDate { get; set; }
        public string? BookDescription { get; set; }
        public string? CoverBase64 { get; set; }
    }
}

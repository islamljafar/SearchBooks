using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElmTest.Domain.Entities
{
    public class Book
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? Author { get; set; }
        public string? PublishDate { get; set; }
        public string? BookDescription { get; set; }
        public string? CoverBase64 { get; set; }
    }
   
}

namespace ElmTest.Shared
{
    public class ApiResponse<T>
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public PagingInfo Paging { get; set; }
    }

    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }

    public class PagingInfo
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

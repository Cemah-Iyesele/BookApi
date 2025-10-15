namespace BookApi.Service_Response
{
    public class ServiceResponse<T> where T : class
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
    }
}

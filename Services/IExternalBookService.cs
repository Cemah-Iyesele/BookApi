namespace BookApi.Services
{
    public interface IExternalBookService
    {
        Task<string> GetBookDescriptionAsync(string isbn);
    }
}

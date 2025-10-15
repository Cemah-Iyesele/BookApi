using BookApi.Models;

namespace BookApi.Services
{
    public interface IExternalBookService
    {
        Task<string> GetBookDescriptionAsync(string isbn);
        Task<List<Book>?> GetBooksFromExternalApiAsync(string query);
    }
}

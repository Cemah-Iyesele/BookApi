using BookApi.Models;
using BookApi.Service_Response;

namespace BookApi.Services
{
    public interface IBookService
    {
        Task<ServiceResponse<Book>> CreateBookAsync(Book payload);
        Task<ServiceResponse<List<Book>>> GetAllBooksAsync();
        Task<ServiceResponse<Book>> GetBookByIdAsync(int id);
        Task<ServiceResponse<Book>> UpdateBookAsync(int id, Book payload);
        Task<ServiceResponse<string>> DeleteBookAsync(int id);
        Task<ServiceResponse<List<Book>>> GetExternalBooksAsync(string query);
    }
}

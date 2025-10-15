using BookApi.Data;
using BookApi.Models;
using BookApi.Service_Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;

namespace BookApi.Services
{
    public sealed class BookService(AppDbContext appDbContext, ILogger<BookService> logger, IExternalBookService externalBookService) : IBookService
    {
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly ILogger<BookService> _logger = logger;
        private readonly IExternalBookService _externalBookService = externalBookService;

        public async Task<ServiceResponse<Book>> CreateBookAsync(Book payload)
        {
            var serviceResponse = new ServiceResponse<Book>();

            try
            {
                var isBookExist = await _appDbContext.Books
                    .FirstOrDefaultAsync(x => x.Title == payload.Title && x.Author == payload.Author);

                if (isBookExist != null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Book already exists.";
                    serviceResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                    serviceResponse.IsSuccess = false;
                    return serviceResponse;
                }

                await _appDbContext.Books.AddAsync(payload);
                await _appDbContext.SaveChangesAsync();

                serviceResponse.Data = payload;
                serviceResponse.Message = "Book created successfully.";
                serviceResponse.StatusCode = (int)HttpStatusCode.OK;
                serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating book: {ex}", ex);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error.";
                serviceResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Book>>> GetAllBooksAsync()
        {
            var serviceResponse = new ServiceResponse<List<Book>>();

            try
            {
                var books = await _appDbContext.Books.AsNoTracking().ToListAsync();

                if (books.Count == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "No books found.";
                    serviceResponse.StatusCode = (int)HttpStatusCode.NoContent;
                    serviceResponse.IsSuccess = false;
                    return serviceResponse;
                }

                serviceResponse.Data = books;
                serviceResponse.Message = "Books retrieved successfully.";
                serviceResponse.StatusCode = (int)HttpStatusCode.OK;
                serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving all books: {ex}", ex);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error.";
                serviceResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Book>> GetBookByIdAsync(int id)
        {
            var serviceResponse = new ServiceResponse<Book>();

            try
            {
                var book = await _appDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

                if (book is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Book not found.";
                    serviceResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    serviceResponse.IsSuccess = false;
                    return serviceResponse;
                }

                serviceResponse.Data = book;
                serviceResponse.Message = "Book retrieved successfully.";
                serviceResponse.StatusCode = (int)HttpStatusCode.OK;
                serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving book by ID: {ex}", ex);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error.";
                serviceResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<Book>> UpdateBookAsync(int id, Book payload)
        {
            var serviceResponse = new ServiceResponse<Book>();

            try
            {
                var book = await _appDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);
                if (book is null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Book not found.";
                    serviceResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    serviceResponse.IsSuccess = false;
                    return serviceResponse;
                }

                book.Title = payload.Title;
                book.Author = payload.Author;
                book.Isbn = payload.Isbn;

                _appDbContext.Books.Update(book);
                await _appDbContext.SaveChangesAsync();

                serviceResponse.Data = book;
                serviceResponse.Message = "Book updated successfully.";
                serviceResponse.StatusCode = (int)HttpStatusCode.OK;
                serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating book: {ex}", ex);
                serviceResponse.Data = null;
                serviceResponse.Message = "Internal Server Error.";
                serviceResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> DeleteBookAsync(int id)
        {
            var serviceResponse = new ServiceResponse<string>();

            try
            {
                var book = await _appDbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

                if (book is null)
                {
                    serviceResponse.Data = string.Empty;
                    serviceResponse.Message = "Book not found.";
                    serviceResponse.StatusCode = (int)HttpStatusCode.NotFound;
                    serviceResponse.IsSuccess = false;
                    return serviceResponse;
                }

                _appDbContext.Books.Remove(book);
                await _appDbContext.SaveChangesAsync();

                serviceResponse.Data = "Book deleted successfully.";
                serviceResponse.Message = "Successful.";
                serviceResponse.StatusCode = (int)HttpStatusCode.OK;
                serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting book: {ex}", ex);
                serviceResponse.Data = string.Empty;
                serviceResponse.Message = "Internal Server Error.";
                serviceResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Book>>> GetExternalBooksAsync(string query)
        {
            var serviceResponse = new ServiceResponse<List<Book>>();

            try
            {
                var externalBooks = await _externalBookService.GetBooksFromExternalApiAsync(query);

                if (externalBooks == null || externalBooks.Count == 0)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "No external books found.";
                    serviceResponse.StatusCode = (int)HttpStatusCode.NoContent;
                    serviceResponse.IsSuccess = false;
                    return serviceResponse;
                }

                serviceResponse.Data = externalBooks;
                serviceResponse.Message = "External books retrieved successfully.";
                serviceResponse.StatusCode = (int)HttpStatusCode.OK;
                serviceResponse.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching external books: {ex}", ex);
                serviceResponse.Data = null;
                serviceResponse.Message = "Error fetching external books.";
                serviceResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                serviceResponse.IsSuccess = false;
            }

            return serviceResponse;
        }
    }
}

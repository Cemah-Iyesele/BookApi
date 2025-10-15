using BookApi.Models;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController(IBookService bookService,IExternalBookService externalBookService, ILogger<BooksController> logger) : ControllerBase
    {
        private readonly IBookService _bookService = bookService;
        private readonly IExternalBookService _externalBookService = externalBookService;
        private readonly ILogger<BooksController> _logger = logger;

        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            _logger.LogInformation("Fetching all books...");
            var serviceResponse = await _bookService.GetAllBooksAsync();
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        [HttpGet("GetBookById/{id}")]
        public async Task<IActionResult> GetBookById(int id)
        {
            _logger.LogInformation("Fetching book with ID: {id}", id);
            var serviceResponse = await _bookService.GetBookByIdAsync(id);
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        [HttpPost("CreateBook")]
        public async Task<IActionResult> CreateBook([FromBody] Book payload)
        {
            _logger.LogInformation("Creating a new book with ISBN: {isbn}", payload.Isbn);
            var serviceResponse = await _bookService.CreateBookAsync(payload);
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        [HttpPut("UpdateBook/{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] Book payload)
        {
            _logger.LogInformation("Updating book with ID: {id}", id);
            var serviceResponse = await _bookService.UpdateBookAsync(id, payload);
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        [HttpDelete("DeleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            _logger.LogInformation("Deleting book with ID: {id}", id);
            var serviceResponse = await _bookService.DeleteBookAsync(id);
            return StatusCode(serviceResponse.StatusCode, serviceResponse);
        }

        [HttpGet("GetExternalBooks")]
        public async Task<IActionResult> GetExternalBooks([FromQuery] string query)
        {
            _logger.LogInformation("Fetching external books for query: {Query}", query);
            var response = await _bookService.GetExternalBooksAsync(query);
            return StatusCode(response.StatusCode, response);
        }


    }
}

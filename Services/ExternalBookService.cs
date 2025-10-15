using System.Text.Json;
using BookApi.Models;
using Microsoft.Extensions.Logging;

namespace BookApi.Services
{
    public class ExternalBookService(HttpClient httpClient, ILogger<ExternalBookService> logger) : IExternalBookService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<ExternalBookService> _logger = logger;

        public async Task<string> GetBookDescriptionAsync(string isbn)
        {
            try
            {
                _logger.LogInformation("Fetching book description for ISBN: {Isbn}", isbn);

                var response = await _httpClient.GetAsync($"volumes?q=isbn:{isbn}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google Books API returned status code: {StatusCode}", response.StatusCode);
                    return "Description not available";
                }

                var content = await response.Content.ReadAsStringAsync();
                using var jsonDocument = JsonDocument.Parse(content);

                var root = jsonDocument.RootElement;

                if (!root.TryGetProperty("items", out var items) || items.GetArrayLength() == 0)
                {
                    _logger.LogInformation("No book found for ISBN: {Isbn}", isbn);
                    return "Description not available";
                }

                
                var firstItem = items[0];
                if (!firstItem.TryGetProperty("volumeInfo", out var volumeInfo))
                {
                    return "Description not available";
                }

                
                if (volumeInfo.TryGetProperty("description", out var descriptionElement) &&
                    descriptionElement.ValueKind != JsonValueKind.Null)
                {
                    var description = descriptionElement.GetString();
                    return !string.IsNullOrEmpty(description) ? description : "Description not available";
                }

                return "Description not available";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching book description for ISBN: {Isbn}", isbn);
                return "Description not available";
            }
        }

       public async Task<List<Book>> GetBooksFromExternalApiAsync(string query)
        {
            try
            {
                _logger.LogInformation("Fetching external books for query: {Query}", query);

                var response = await _httpClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?q={query}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Google Books API returned: {StatusCode}", response.StatusCode);
                    return [];
                }

                var content = await response.Content.ReadAsStringAsync();
                using var jsonDocument = JsonDocument.Parse(content);

                var root = jsonDocument.RootElement;
                if (!root.TryGetProperty("items", out var items) || items.GetArrayLength() == 0)
                {
                    _logger.LogInformation("No books found for query: {Query}", query);
                    return [];
                }

                var books = new List<Book>();

                foreach (var item in items.EnumerateArray())
                {
                    if (!item.TryGetProperty("volumeInfo", out var volumeInfo))
                        continue;

                    var title = volumeInfo.GetProperty("title").GetString();
                    var authors = volumeInfo.TryGetProperty("authors", out var authorsElement)
                        ? string.Join(", ", authorsElement.EnumerateArray().Select(a => a.GetString()))
                        : "Unknown";

                    var isbn = volumeInfo.TryGetProperty("industryIdentifiers", out var identifiers)
                        ? identifiers[0].GetProperty("identifier").GetString()
                        : "N/A";

                    var description = volumeInfo.TryGetProperty("description", out var descElement)
                        ? descElement.GetString()
                        : "Description not available";

                    var book = new Book
                    {
                        Title = title ?? "Untitled",
                        Author = authors,
                        Isbn = isbn,
                        Description = description
                    };

                    books.Add(book);
                }

                return books;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching external books for query: {Query}", query);
                return [];
            }
        }
    }
}
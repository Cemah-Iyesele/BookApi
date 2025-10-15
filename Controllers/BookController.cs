using BookApi.Data;
using BookApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController(AppDbContext context, IExternalBookService externalBookService,
                          ILogger<BookController> logger) : ControllerBase
    {
        private readonly AppDbContext _context = context;
        private readonly IExternalBookService _externalBookService = externalBookService;
        private readonly ILogger<BookController> _logger = logger;
    }
}

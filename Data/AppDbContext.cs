using Microsoft.EntityFrameworkCore;
using BookApi.Model;

namespace BookApi.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
    }
}

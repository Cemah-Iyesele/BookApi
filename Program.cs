using BookApi.Services;
using BookApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(static options =>
    options.UseInMemoryDatabase("BooksDB"));

builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddHttpClient<IExternalBookService, ExternalBookService>(client =>
{
    client.BaseAddress = new Uri("https://www.googleapis.com/books/v1/");
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddLogging();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

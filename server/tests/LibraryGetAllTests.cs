using api.Services;
using efscaffold;

namespace tests;

public class LibraryGetAllTests(ILibraryService libraryService, MyDbContext ctx)
{
    [Fact]
    public async Task GetAuthors_ReturnsAll()
    {
        var author = new Author
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Alice",
            Createdat = DateTime.UtcNow
        };
        ctx.Authors.Add(author);
        ctx.SaveChanges();

        var result = await libraryService.GetAuthors();
        Assert.Contains(result, a => a.Id == author.Id);
    }

    [Fact]
    public async Task GetGenres_ReturnsAll()
    {
        var genre = new Genre
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Adventure",
            Createdat = DateTime.UtcNow
        };
        ctx.Genres.Add(genre);
        ctx.SaveChanges();

        var result = await libraryService.GetGenres();
        Assert.Contains(result, g => g.Id == genre.Id);
    }

    [Fact]
    public async Task GetBooks_ReturnsAll()
    {
        var book = new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = "Book A",
            Pages = 123,
            Createdat = DateTime.UtcNow
        };
        ctx.Books.Add(book);
        ctx.SaveChanges();

        var result = await libraryService.GetBooks();
        Assert.Contains(result, b => b.Id == book.Id);
    }
}
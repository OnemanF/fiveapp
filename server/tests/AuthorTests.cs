using api.DTOs.Requests;
using api.Services;
using efscaffold;
using Microsoft.EntityFrameworkCore;
using api;

namespace tests;

public class AuthorTests(ILibraryService libraryService, MyDbContext ctx, Seeder seeder)
{
    [Fact]
    public async Task CreateAuthor_CreatesSuccessfully()
    {
        await seeder.Seed();

        var dto = new CreateAuthorRequestDto { Name = "John Doe" };
        var result = await libraryService.CreateAuthor(dto);

        Assert.NotNull(result);
        Assert.Equal("John Doe", result.Name);
        Assert.True(ctx.Authors.Any(a => a.Name == "John Doe"));
    }

    [Fact]
    public async Task UpdateAuthor_CanUpdateAuthorPropertiesAndAddBooks()
    {
        await seeder.Seed();
        var author = ctx.Authors.First();

        var updateRequest = new UpdateAuthorRequestDto
        {
            AuthorIdForLookup = author.Id,
            BooksIds = [ctx.Books.First().Id],
            NewName = "Updated Name"
        };

        var result = await libraryService.UpdateAuthor(updateRequest);

        // ✅ Fixed: AuthorDto uses BooksIds (not Books)
        Assert.Equal("Updated Name", result.Name);
        Assert.Single(result.BooksIds);

        // ✅ Cleaner xUnit assertion (was Assert.True(...))
        var updatedBook = ctx.Books
            .Include(b => b.Authors)
            .First();

        Assert.Contains(updatedBook.Authors, a => a.Name == "Updated Name");
    }

    [Fact]
    public async Task DeleteAuthor_RemovesSuccessfully()
    {
        await seeder.Seed();
        var author = ctx.Authors.First();

        await libraryService.DeleteAuthor(author.Id);
        Assert.False(ctx.Authors.Any(a => a.Id == author.Id));
    }

    [Fact]
    public async Task DeleteAuthor_ThrowsIfNotFound()
    {
        await Assert.ThrowsAnyAsync<Exception>(
            () => libraryService.DeleteAuthor("nonexisting-id"));
    }
}
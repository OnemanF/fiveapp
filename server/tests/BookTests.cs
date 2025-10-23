using System.ComponentModel.DataAnnotations;
using api.DTOs.Requests;
using api.Services;
using efscaffold;
using Microsoft.EntityFrameworkCore;
using api;

namespace tests;

public class BookTests(MyDbContext ctx, ILibraryService libraryService, Seeder seeder)
{
    [Fact]
    public async Task GetBooks_ReturnsAllBooks()
    {
        await seeder.Seed();
        var books = await libraryService.GetBooks();

        Assert.NotEmpty(books);
        Assert.Equal(ctx.Books.First().Id, books.First().Id);
    }

    [Fact]
    public async Task CreateBook_Success()
    {
        var dto = new CreateBookRequestDto
        {
            Title = "New Book",
            Pages = 100
        };

        var result = await libraryService.CreateBook(dto);

        Assert.NotNull(result);
        Assert.Equal("New Book", result.Title);
        Assert.Equal(100, result.Pages);
    }

    [Fact]
    public async Task UpdateBook_CanChangeFieldsAndRelations()
    {
        await seeder.Seed();

        var dto = new UpdateBookRequestDto
        {
            BookIdForLookupReference = ctx.Books.First().Id,
            NewTitle = "Updated Title",
            NewPageCount = 200,
            GenreId = ctx.Genres.First().Id,
            AuthorsIds = new List<string> { ctx.Authors.First().Id }
        };

        var result = await libraryService.UpdateBook(dto);
        

        Assert.Equal("Updated Title", result.Title);
        Assert.Equal(200, result.Pages);
        Assert.NotNull(result.Genre);
        Assert.Single(result.AuthorsIds);
    }

    [Fact]
    public async Task DeleteBook_RemovesSuccessfully()
    {
        await seeder.Seed();
        var book = ctx.Books.First();

        await libraryService.DeleteBook(book.Id);
        Assert.False(ctx.Books.Any(b => b.Id == book.Id));
    }

    [Fact]
    public async Task CreateBook_ThrowsOnInvalidData()
    {
        var invalidDto = new CreateBookRequestDto
        {
            Title = "",
            Pages = 0
        };

        await Assert.ThrowsAnyAsync<ValidationException>(() => libraryService.CreateBook(invalidDto));
    }

    [Fact]
    public async Task UpdateBook_ThrowsIfInvalid()
    {
        await seeder.Seed();

        var invalidDto = new UpdateBookRequestDto
        {
            BookIdForLookupReference = ctx.Books.First().Id,
            NewTitle = "",
            NewPageCount = 0,
            AuthorsIds = new List<string> { ctx.Authors.First().Id }
        };

        await Assert.ThrowsAnyAsync<ValidationException>(() => libraryService.UpdateBook(invalidDto));
    }
}

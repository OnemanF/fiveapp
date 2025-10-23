using api.DTOs.Requests;
using api.Services;
using efscaffold;
using Microsoft.EntityFrameworkCore;
using api;

namespace tests;

public class GenreTests(ILibraryService libraryService, MyDbContext ctx, Seeder seeder)
{
    [Fact]
    public async Task CreateGenre_CreatesSuccessfully()
    {
        await seeder.Seed();

        var dto = new CreateGenreDto { Name = "Sci-Fi" };
        var result = await libraryService.CreateGenre(dto);

        Assert.NotNull(result);
        Assert.Equal("Sci-Fi", result.Name);
        Assert.True(ctx.Genres.Any(g => g.Name == "Sci-Fi"));
    }

    [Fact]
    public async Task UpdateGenre_UpdatesSuccessfully()
    {
        await seeder.Seed();
        var genre = ctx.Genres.First();

        var updateDto = new UpdateGenreRequestDto
        {
            IdToLookupBy = genre.Id,
            NewName = "UpdatedGenre"
        };

        var updated = await libraryService.UpdateGenre(updateDto);
        Assert.Equal("UpdatedGenre", updated.Name);
    }

    [Fact]
    public async Task DeleteGenre_RemovesSuccessfully()
    {
        await seeder.Seed();
        var genre = ctx.Genres.First();

        await libraryService.DeleteGenre(genre.Id);
        Assert.False(ctx.Genres.Any(g => g.Id == genre.Id));
    }

    [Fact]
    public async Task DeleteGenre_ThrowsIfNotFound()
    {
        await Assert.ThrowsAnyAsync<Exception>(
            async () => await libraryService.DeleteGenre("nonexistent-id"));
    }
}
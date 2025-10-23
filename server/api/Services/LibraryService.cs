// File: api/Services/LibraryService.cs
using System.ComponentModel.DataAnnotations;
using api.DTOs;
using api.DTOs.Requests;
using efscaffold;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class LibraryService(MyDbContext ctx) : ILibraryService
{
    // ---------- AUTHORS ----------
    public async Task<List<AuthorDto>> GetAuthors() =>
        await ctx.Authors.Select(a => new AuthorDto(a)).ToListAsync();

    public async Task<AuthorDto> CreateAuthor(CreateAuthorRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var author = new Author
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name
        };

        ctx.Authors.Add(author);
        await ctx.SaveChangesAsync();
        return new AuthorDto(author);
    }

    public async Task<AuthorDto> UpdateAuthor(UpdateAuthorRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var author = await ctx.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == dto.AuthorIdForLookup)
            ?? throw new InvalidOperationException("Author not found.");

        author.Name = dto.NewName;

        // Refresh associated books
        author.Books.Clear();
        if (dto.BooksIds.Count > 0)
        {
            var books = await ctx.Books
                .Where(b => dto.BooksIds.Contains(b.Id))
                .ToListAsync();
            foreach (var b in books)
                author.Books.Add(b);
        }

        await ctx.SaveChangesAsync();
        return new AuthorDto(author);
    }

    public async Task<AuthorDto> DeleteAuthor(string authorId)
    {
        var author = await ctx.Authors.FirstOrDefaultAsync(a => a.Id == authorId)
            ?? throw new InvalidOperationException("Author not found.");

        ctx.Authors.Remove(author);
        await ctx.SaveChangesAsync();
        return new AuthorDto(author);
    }

    // ---------- GENRES ----------
    public async Task<List<GenreDto>> GetGenres() =>
        await ctx.Genres.Select(g => new GenreDto(g)).ToListAsync();

    public async Task<GenreDto> CreateGenre(CreateGenreDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var genre = new Genre
        {
            Id = Guid.NewGuid().ToString(),
            Name = dto.Name
        };

        ctx.Genres.Add(genre);
        await ctx.SaveChangesAsync();
        return new GenreDto(genre);
    }

    public async Task<GenreDto> UpdateGenre(UpdateGenreRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var genre = await ctx.Genres.FirstOrDefaultAsync(g => g.Id == dto.IdToLookupBy)
            ?? throw new InvalidOperationException("Genre not found.");

        genre.Name = dto.NewName;
        await ctx.SaveChangesAsync();
        return new GenreDto(genre);
    }

    public async Task<GenreDto> DeleteGenre(string genreId)
    {
        var genre = await ctx.Genres.FirstOrDefaultAsync(g => g.Id == genreId)
            ?? throw new InvalidOperationException("Genre not found.");

        ctx.Genres.Remove(genre);
        await ctx.SaveChangesAsync();
        return new GenreDto(genre);
    }

    // ---------- BOOKS ----------
    public async Task<List<BookDto>> GetBooks() =>
        await ctx.Books.Include(b => b.Authors)
                       .Include(b => b.Genre)
                       .Select(b => new BookDto(b))
                       .ToListAsync();

    public async Task<BookDto> CreateBook(CreateBookRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var book = new Book
        {
            Id = Guid.NewGuid().ToString(),
            Title = dto.Title,
            Pages = dto.Pages,
            Createdat = DateTime.UtcNow
        };

        if (!string.IsNullOrEmpty(dto.GenreId))
            book.Genre = await ctx.Genres.FirstOrDefaultAsync(g => g.Id == dto.GenreId);

        if (dto.AuthorsIds?.Count > 0)
        {
            var authors = await ctx.Authors
                .Where(a => dto.AuthorsIds.Contains(a.Id))
                .ToListAsync();
            book.Authors = authors;
        }

        ctx.Books.Add(book);
        await ctx.SaveChangesAsync();
        return new BookDto(book);
    }

    public async Task<BookDto> UpdateBook(UpdateBookRequestDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), true);

        var book = await ctx.Books
            .Include(b => b.Authors)
            .Include(b => b.Genre)
            .FirstOrDefaultAsync(b => b.Id == dto.BookIdForLookupReference)
            ?? throw new InvalidOperationException("Book not found.");

        book.Title = dto.NewTitle;
        book.Pages = dto.NewPageCount;
        book.Genre = dto.GenreId != null
            ? await ctx.Genres.FirstOrDefaultAsync(g => g.Id == dto.GenreId)
            : null;

        book.Authors.Clear();
        if (dto.AuthorsIds?.Count > 0)
        {
            var authors = await ctx.Authors
                .Where(a => dto.AuthorsIds.Contains(a.Id))
                .ToListAsync();
            foreach (var a in authors)
                book.Authors.Add(a);
        }

        await ctx.SaveChangesAsync();
        return new BookDto(book);
    }

    public async Task<BookDto> DeleteBook(string bookId)
    {
        var book = await ctx.Books.FirstOrDefaultAsync(b => b.Id == bookId)
            ?? throw new InvalidOperationException("Book not found.");

        ctx.Books.Remove(book);
        await ctx.SaveChangesAsync();
        return new BookDto(book);
    }

    public async Task<List<BookDto>> GetBooksPaginated(int skip, int take)
    {
        var query = ctx.Books.Include(b => b.Authors).Include(b => b.Genre);
        return await query.Skip(skip).Take(take).Select(b => new BookDto(b)).ToListAsync();
    }

    public async Task<List<BookDto>> FetchBooksAsync(FetchBooksRequestDto dto)
    {
        var query = ctx.Books
            .Include(b => b.Genre)
            .Include(b => b.Authors)
            .AsQueryable();

        query = dto.OrderBy switch
        {
            BookOrdering.NameAsc => query.OrderBy(b => b.Title),
            BookOrdering.NameDesc => query.OrderByDescending(b => b.Title),
            BookOrdering.LongestTitle => query.OrderByDescending(b => b.Title.Length),
            BookOrdering.MostPages => query.OrderByDescending(b => b.Pages),
            BookOrdering.GenreName => query.OrderBy(b => b.Genre != null ? b.Genre.Name : ""),
            BookOrdering.MostRecentlyCreated => query.OrderByDescending(b => b.Createdat),
            BookOrdering.MostProductiveAuthor => query
                .OrderByDescending(b => b.Authors.Max(a => a.Books.Count))
                .ThenBy(b => b.Title),
            _ => query.OrderBy(b => b.Title)
        };

        dto.Skip = Math.Max(0, dto.Skip);
        dto.Take = dto.Take <= 0 ? 10 : dto.Take;

        var books = await query
            .Skip(dto.Skip)
            .Take(dto.Take)
            .Select(b => new BookDto(b))
            .ToListAsync();

        return books;
    }
}

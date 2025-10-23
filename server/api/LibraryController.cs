using api.DTOs;
using api.DTOs.Requests;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
[Route("api/library")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    // ---------- AUTHORS ----------
    [HttpGet("authors")]
    public async Task<ActionResult<List<AuthorDto>>> GetAuthors()
        => Ok(await _libraryService.GetAuthors());

    [HttpPost("authors")]
    public async Task<ActionResult<AuthorDto>> CreateAuthor([FromBody] CreateAuthorRequestDto dto)
        => Ok(await _libraryService.CreateAuthor(dto));

    [HttpDelete("authors/{authorId}")]
    public async Task<ActionResult<AuthorDto>> DeleteAuthor(string authorId)
        => Ok(await _libraryService.DeleteAuthor(authorId));

    // ---------- BOOKS ----------
    [HttpGet("books")]
    public async Task<ActionResult<List<BookDto>>> GetBooks()
        => Ok(await _libraryService.GetBooks());

    [HttpPost("books")]
    public async Task<ActionResult<BookDto>> CreateBook([FromBody] CreateBookRequestDto dto)
        => Ok(await _libraryService.CreateBook(dto));

    [HttpPut("books")]
    public async Task<ActionResult<BookDto>> UpdateBook([FromBody] UpdateBookRequestDto dto)
        => Ok(await _libraryService.UpdateBook(dto));

    [HttpDelete("books/{bookId}")]
    public async Task<ActionResult<BookDto>> DeleteBook(string bookId)
        => Ok(await _libraryService.DeleteBook(bookId));

    // ---------- GENRES ----------
    [HttpGet("genres")]
    public async Task<ActionResult<List<GenreDto>>> GetGenres()
        => Ok(await _libraryService.GetGenres());

    [HttpPost("genres")]
    public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] CreateGenreDto dto)
        => Ok(await _libraryService.CreateGenre(dto));

    [HttpPut("genres")]
    public async Task<ActionResult<GenreDto>> UpdateGenre([FromBody] UpdateGenreRequestDto dto)
        => Ok(await _libraryService.UpdateGenre(dto));

    [HttpDelete("genres/{genreId}")]
    public async Task<ActionResult<GenreDto>> DeleteGenre(string genreId)
        => Ok(await _libraryService.DeleteGenre(genreId));

    // ---------- PAGINATION & FETCH ----------
    [HttpGet("books/paginated")]
    public async Task<IActionResult> GetBooksPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        int skip = (page - 1) * pageSize;
        var books = await _libraryService.GetBooksPaginated(skip, pageSize);
        return Ok(books);
    }

    [HttpPost("books/fetch")]
    public async Task<IActionResult> FetchBooks([FromBody] FetchBooksRequestDto dto)
    {
        var books = await _libraryService.FetchBooksAsync(dto);
        return Ok(books);
    }
}

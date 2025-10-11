using api.DTOs;
using api.DTOs.Requests;
using api.Services;
using Microsoft.AspNetCore.Mvc;

namespace api;

[ApiController]
[Route("api/[controller]")]
public class LibraryController : ControllerBase
{
    private readonly ILibraryService _libraryService;

    public LibraryController(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    [HttpGet(nameof(GetAuthors))]
    public async Task<List<AuthorDto>> GetAuthors() => await _libraryService.GetAuthors();

    [HttpGet(nameof(GetBooks))]
    public async Task<List<BookDto>> GetBooks() => await _libraryService.GetBooks();

    [HttpGet(nameof(GetGenres))]
    public async Task<List<GenreDto>> GetGenres() => await _libraryService.GetGenres();

    [HttpPost(nameof(CreateBook))]
    public async Task<BookDto> CreateBook([FromBody] CreateBookRequestDto dto)
        => await _libraryService.CreateBook(dto);

    [HttpPut(nameof(UpdateBook))]
    public async Task<BookDto> UpdateBook([FromBody] UpdateBookRequestDto dto)
        => await _libraryService.UpdateBook(dto);

    [HttpDelete(nameof(DeleteBook))]
    public async Task<BookDto> DeleteBook([FromQuery] string bookId)
        => await _libraryService.DeleteBook(bookId);

    // ✅ Simple pagination endpoint
    [HttpGet("books/paginated")]
    public async Task<IActionResult> GetBooksPaginated(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
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
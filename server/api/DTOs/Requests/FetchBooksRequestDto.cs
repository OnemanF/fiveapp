namespace api.DTOs.Requests;

public class FetchBooksRequestDto
{
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;
    public BookOrdering OrderBy { get; set; } = BookOrdering.NameAsc;
}
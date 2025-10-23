using efscaffold;

namespace api.DTOs;

public class AuthorDto
{
    public AuthorDto(Author entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        CreatedAt = entity.Createdat;
        BooksIds = entity.Books?.Select(b => b.Id).ToList() ?? new List<string>();
    }

    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }

    // ✅ only book IDs (no nested DTOs)
    public List<string> BooksIds { get; set; } = new();
}
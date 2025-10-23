using efscaffold;

namespace api.DTOs;

public class GenreDto
{
    public GenreDto(Genre entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        CreatedAt = entity.Createdat;
        Books = entity.Books?.Select(b => b.Id).ToList() ?? new List<string>();
    }

    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public List<string> Books { get; set; } = new();
}
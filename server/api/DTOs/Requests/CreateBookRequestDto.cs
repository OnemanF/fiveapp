using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Requests
{
    public record CreateBookRequestDto
    {
        [Required]
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int Pages { get; set; }

        // Optional: link authors when creating
        public List<string> AuthorsIds { get; set; } = new();

        // Optional: link genre
        public string? GenreId { get; set; }
    }
}
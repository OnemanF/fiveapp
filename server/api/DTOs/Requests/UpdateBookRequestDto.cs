using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Requests
{
    public record UpdateBookRequestDto
    {
        [Required]
        [MinLength(1)]
        public string BookIdForLookupReference { get; set; } = string.Empty;

        [Required]
        [MinLength(1)]
        public string NewTitle { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int NewPageCount { get; set; }

        [Required]
        public List<string> AuthorsIds { get; set; } = new();

        public string? GenreId { get; set; }
    }
}
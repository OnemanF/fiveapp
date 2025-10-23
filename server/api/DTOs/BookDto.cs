using System;
using System.Collections.Generic;
using System.Linq;
using efscaffold;

namespace api.DTOs
{
    public class BookDto
    {
        public BookDto(Book entity)
        {
            Id = entity.Id;
            Title = entity.Title;
            Pages = entity.Pages;
            Createdat = entity.Createdat;

            // Link genre if exists
            if (entity.Genre != null)
                Genre = new GenreDto(entity.Genre);

            // List of authors linked to this book
            AuthorsIds = entity.Authors?.Select(a => a.Id).ToList() ?? new List<string>();
            Authors = entity.Authors?.Select(a => new AuthorDto(a)).ToList() ?? new List<AuthorDto>();
        }

        public string Id { get; set; } = null!;

        public string Title { get; set; } = null!;

        public int Pages { get; set; }

        public DateTime? Createdat { get; set; }

        public virtual GenreDto? Genre { get; set; }

        public virtual ICollection<string> AuthorsIds { get; set; } = new List<string>();

        // Optional: full author objects for UI display
        public virtual ICollection<AuthorDto> Authors { get; set; } = new List<AuthorDto>();
    }
}
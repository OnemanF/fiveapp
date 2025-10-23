using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using efscaffold;

namespace api
{
    public class Seeder
    {
        private readonly MyDbContext _ctx;

        public Seeder(MyDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task Seed()
        {
            // Clear previous data
            _ctx.Books.RemoveRange(_ctx.Books);
            _ctx.Authors.RemoveRange(_ctx.Authors);
            _ctx.Genres.RemoveRange(_ctx.Genres);
            await _ctx.SaveChangesAsync();

            // Add author
            var author = new Author
            {
                Id = "1",
                Name = "Bob",
                Createdat = DateTime.UtcNow
            };
            _ctx.Authors.Add(author);

            // Add genre
            var genre = new Genre
            {
                Id = "1",
                Name = "Thriller",
                Createdat = DateTime.UtcNow
            };
            _ctx.Genres.Add(genre);

            // Add book with author and genre
            var book = new Book
            {
                Id = "1",
                Title = "Bobs book",
                Pages = 42,
                Createdat = DateTime.UtcNow,
                Authors = new List<Author> { author },
                Genre = genre
            };
            _ctx.Books.Add(book);

            await _ctx.SaveChangesAsync();
        }
    }
}
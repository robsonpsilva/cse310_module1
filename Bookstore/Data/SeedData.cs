namespace Bookstore.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(BookstoreDbContext db)
        {
            if (db.Books.Any()) return;

            var books = new List<Book>
            {
                new() { Title = "Clean Code", Author = "Robert C. Martin", ISBN = "9780132350884" },
                new() { Title = "The Pragmatic Programmer", Author = "Andrew Hunt", ISBN = "9780201616224" },
                new() { Title = "Design Patterns", Author = "Erich Gamma", ISBN = "9780201633610" },
                new() { Title = "Refactoring", Author = "Martin Fowler", ISBN = "9780201485677" },
                new() { Title = "Introduction to Algorithms", Author = "Cormen et al.", ISBN = "9780262033848" }
            };

            db.Books.AddRange(books);
            await db.SaveChangesAsync();
        }
    }
}

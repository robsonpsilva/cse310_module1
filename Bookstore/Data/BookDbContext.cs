using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    // The database context class for the Bookstore application.
    // It manages the connection to the database and maps entities to tables.
    public class BookstoreDbContext : DbContext
    {
        // Constructor that passes configuration options (like connection string) to the base DbContext.
        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options) { }

        // Represents the "Books" table in the database.
        // Each Book entity corresponds to a row in this table.
        public DbSet<Book> Books => Set<Book>();

        // Configures additional model settings and constraints.
        // This method is called when the model is being created.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Create an index on the Title column to improve search performance.
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);

            // Create an index on the Author column to improve search performance.
            modelBuilder.Entity<Book>().HasIndex(b => b.Author);

            // Create a unique index on the ISBN column to ensure no duplicate ISBNs are stored.
            modelBuilder.Entity<Book>().HasIndex(b => b.ISBN).IsUnique();
        }
    }
}

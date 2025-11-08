using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    public class BookstoreDbContext : DbContext
    {
        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options) { }

        public DbSet<Book> Books => Set<Book>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasIndex(b => b.Title);
            modelBuilder.Entity<Book>().HasIndex(b => b.Author);
            modelBuilder.Entity<Book>().HasIndex(b => b.ISBN).IsUnique();
        }
    }
}

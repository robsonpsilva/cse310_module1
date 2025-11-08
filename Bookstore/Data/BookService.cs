using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    // Service class that inherits from BaseService.
    public class BookService : BaseService
    {
        
        // Constructor: Calls the base class constructor (BaseService)
        // to initialize the _context.
        public BookService(BookstoreDbContext context) : base(context)
        {
            // The body of the constructor is empty, as the work was done by the base.
        }

        // The following methods continue to work perfectly using the inherited '_context'.

        // Retrieves all books from the database, ordered alphabetically by title.
        public async Task<List<Book>> GetBooksAsync()
        {
            return await _context.Books.OrderBy(b => b.Title).ToListAsync();
        }

        // Searches for books by title, author, or ISBN.
        public async Task<List<Book>> SearchAsync(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return await GetBooksAsync();

            term = term.Trim().ToLower();

            return await _context.Books
                .Where(b =>
                    EF.Functions.Like(b.Title.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(b.Author.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(b.ISBN.ToLower(), $"%{term}%"))
                .OrderBy(b => b.Title)
                .ToListAsync();
        }

        // Marks a book as borrowed.
        public async Task BorrowAsync(int bookId, string borrowerName)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book is null) throw new InvalidOperationException("Book not found.");
            if (!book.IsAvailable) throw new InvalidOperationException("Book is already borrowed.");

            book.IsAvailable = false;
            book.BorrowerName = borrowerName;
            book.BorrowDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // Marks a book as returned.
        public async Task ReturnAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book is null) throw new InvalidOperationException("Book not found.");
            if (book.IsAvailable) throw new InvalidOperationException("Book is not borrowed.");

            book.IsAvailable = true;
            book.BorrowerName = null;
            book.BorrowDate = null;

            await _context.SaveChangesAsync();
        }
    }
}
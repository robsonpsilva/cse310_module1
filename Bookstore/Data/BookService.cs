using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    // Service class that encapsulates business logic for managing books.
    // Provides methods for retrieving, searching, borrowing, and returning books.
    public class BookService
    {
        // Database context used to interact with the underlying SQLite (or other) database.
        private readonly BookstoreDbContext _context;

        // Constructor that receives the DbContext via dependency injection.
        public BookService(BookstoreDbContext context)
        {
            _context = context;
        }

        // Retrieves all books from the database, ordered alphabetically by title.
        public async Task<List<Book>> GetBooksAsync()
        {
            return await _context.Books.OrderBy(b => b.Title).ToListAsync();
        }

        // Searches for books by title, author, or ISBN.
        // If the search term is empty, returns all books.
        // Uses EF.Functions.Like for case-insensitive pattern matching.
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
        // Validates that the book exists and is currently available.
        // Updates availability, borrower name, and borrow date.
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
        // Validates that the book exists and is currently borrowed.
        // Resets availability, borrower name, and borrow date.
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

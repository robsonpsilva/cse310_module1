using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    public class BookService
    {
        private readonly BookstoreDbContext _context;

        public BookService(BookstoreDbContext context)
        {
            _context = context;
        }

        // Get all books
        public async Task<List<Book>> GetBooksAsync()
        {
            return await _context.Books.OrderBy(b => b.Title).ToListAsync();
        }

        // Search by title, author, or ISBN
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

        // Borrow a book
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

        // Return a book
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

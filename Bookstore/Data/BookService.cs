using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// NOTE: This code assumes the existence of the following dependencies:
// 1. 'BaseService' class (abstract class handling _context initialization).
// 2. 'BookstoreDbContext' class (derived from DbContext).
// 3. 'Book' entity class (containing properties like Title, Author, BookId, IsAvailable, BorrowerName, and importantly, BorrowDate).

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

        // --- READ/QUERY METHODS (Retrieve) ---

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

        // Query demonstrating filtering by a date range (Borrowed books).
        // This method builds the necessary SQL WHERE clause for the date filter.
        public async Task<List<Book>> GetBooksBorrowedInDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            // Ensures the date range is inclusive. We also filter by IsAvailable=false, 
            // as only borrowed books should have a BorrowDate.
            return await _context.Books
                .Where(b => 
                    b.IsAvailable == false &&
                    b.BorrowDate.HasValue &&
                    b.BorrowDate.Value.Date >= startDate.Date && // Compare dates only
                    b.BorrowDate.Value.Date <= endDate.Date)     // Compare dates only
                .OrderBy(b => b.BorrowDate)
                .ToListAsync();
        }

        // --- CREATE METHOD (Insert) ---

        // Inserts a new book record into the database.
        public async Task AddBookAsync(Book newBook)
        {
            if (newBook is null) throw new ArgumentNullException(nameof(newBook));
            
            // Set initial state (assuming it's a new book)
            newBook.IsAvailable = true;
            newBook.BorrowDate = null;
            newBook.BorrowerName = null;

            // This command prepares the SQL INSERT statement.
            _context.Books.Add(newBook);
            
            // Submits the command and receives the result (number of affected rows).
            await _context.SaveChangesAsync();
        }

        // --- UPDATE METHODS (Modify) ---

        // Finds an existing book and updates its non-transactional details (Title, Author, ISBN).
        public async Task UpdateBookDetailsAsync(Book updatedBook)
        {
            var existingBook = await _context.Books.FindAsync(updatedBook.BookId);
            if (existingBook is null) throw new InvalidOperationException("Book not found for update.");
            
            // Applying changes to the tracked entity
            existingBook.Title = updatedBook.Title;
            existingBook.Author = updatedBook.Author;
            existingBook.ISBN = updatedBook.ISBN;
            
            // This command prepares the SQL UPDATE statement.
            await _context.SaveChangesAsync();
        }
        
        // Marks a book as borrowed. (Existing Update method)
        public async Task BorrowAsync(int bookId, string borrowerName)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book is null) throw new InvalidOperationException("Book not found.");
            if (!book.IsAvailable) throw new InvalidOperationException("Book is already borrowed.");

            book.IsAvailable = false;
            book.BorrowerName = borrowerName;
            book.BorrowDate = DateTime.UtcNow; // Uses a date/time column

            await _context.SaveChangesAsync();
        }

        // Marks a book as returned. (Existing Update method)
        public async Task ReturnAsync(int bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book is null) throw new InvalidOperationException("Book not found.");
            if (book.IsAvailable) throw new InvalidOperationException("Book is not borrowed.");

            book.IsAvailable = true;
            book.BorrowerName = null;
            book.BorrowDate = null; // Clears the date/time column

            await _context.SaveChangesAsync();
        }
        
        // --- DELETE METHOD (Delete) ---

        // Deletes a book record from the database.
        public async Task DeleteBookAsync(int bookId)
        {
            var bookToDelete = await _context.Books.FindAsync(bookId);
            if (bookToDelete is null) return; // Book not found, nothing to do.

            // This command prepares the SQL DELETE statement.
            _context.Books.Remove(bookToDelete);
            
            // Submits the command and receives the result.
            await _context.SaveChangesAsync();
        }
    }
}
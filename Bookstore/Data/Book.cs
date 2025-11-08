using System.ComponentModel.DataAnnotations;

namespace Bookstore.Data
{
    // Represents the main entity in the system: a book in the bookstore
    public class Book
    {
        // Unique identifier for the book (primary key in the database)
        public int BookId { get; set; }

        // Book title - required and limited to 200 characters
        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        // Author name - required and limited to 150 characters
        [Required, MaxLength(150)]
        public string Author { get; set; } = string.Empty;

        // ISBN code - required and limited to 30 characters
        [Required, MaxLength(30)]
        public string ISBN { get; set; } = string.Empty;

        // Indicates whether the book is available for borrowing
        // true = available, false = borrowed
        public bool IsAvailable { get; set; } = true;

        // Name of the person who borrowed the book (null if available)
        public string? BorrowerName { get; set; }

        // Date when the book was borrowed (null if available)
        public DateTime? BorrowDate { get; set; }
    }
}

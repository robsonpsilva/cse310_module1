using System.ComponentModel.DataAnnotations;

namespace Bookstore.Data
{
    public class Book
    {
        public int BookId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string Author { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        public string ISBN { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public string? BorrowerName { get; set; }
        public DateTime? BorrowDate { get; set; }
    }
}

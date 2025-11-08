using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    // 🗄️ Classe Base para Serviços que dependem do DbContext
    public class BaseService
    {
        // Torna o contexto protegido para que classes derivadas (como BookService) possam acessá-lo.
        protected readonly BookstoreDbContext _context;

        // Construtor que recebe o DbContext via injeção de dependência.
        public BaseService(BookstoreDbContext context)
        {
            _context = context;
        }
    }
}
using Bookstore.Data;
using Microsoft.EntityFrameworkCore;
using Bookstore.Components.Pages; // garante que o App.razor seja encontrado

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte ao Blazor Server (modo interativo)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configura o DbContext com SQLite
builder.Services.AddDbContext<BookstoreDbContext>(options =>
    options.UseSqlite("Data Source=bookstore.db"));

// Registra o serviço de livros
builder.Services.AddScoped<BookService>();

var app = builder.Build();

// Aplica migrações e carrega dados fictícios
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BookstoreDbContext>();
    db.Database.Migrate();
    await SeedData.InitializeAsync(db);
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Mapeia o host page App.razor corretamente
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

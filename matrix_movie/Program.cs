using matrix_movie.Data;
using matrix_movie.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TuoProgetto.Data;

var builder = WebApplication.CreateBuilder(args);

// Connessione al database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

// Configurazione cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

var app = builder.Build();

// Inizializza ruoli (se hai RoleInitializer)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleInitializer.InitializeAsync(services);
}

// 🌱 POPOLAMENTO FILM — aggiungi questa parte PRIMA di app.Run()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated(); // Garantisce che il DB esista

    if (!db.Movies.Any())
    {
        db.Movies.AddRange(new List<Movie>
        {
            new Movie
            {
                Title = "The Matrix",
                ImageUrl = "https://image.tmdb.org/t/p/w500/f89U3ADr1oiB1s9GkdPOEpXUk5H.jpg",
                Genre = "Sci-Fi",
                Year = 1999,
                Description = "Un hacker scopre la verità dietro il mondo virtuale in cui vive.",
                Plot = "Neo, un abile hacker, scopre che la realtà è una simulazione creata dalle macchine per controllare gli esseri umani. Con l'aiuto di Morpheus e Trinity, decide di combattere contro il sistema artificiale che tiene prigioniera l'umanità."
            },
            new Movie
            {
                Title = "Inception",
                ImageUrl = "https://image.tmdb.org/t/p/w500/9gk7adHYeDvHkCSEqAvQNLV5Uge.jpg",
                Genre = "Sci-Fi",
                Year = 2010,
                Description = "Un ladro entra nei sogni delle persone per rubare segreti nascosti.",
                Plot = "Dom Cobb è un esperto nell'arte dell'estrazione: rubare segreti dal subconscio mentre si sogna. Gli viene proposto un compito impossibile: impiantare un'idea nella mente di un erede industriale, affrontando così il proprio passato."
            },
            new Movie
            {
                Title = "Interstellar",
                ImageUrl = "https://image.tmdb.org/t/p/w500/gEU2QniE6E77NI6lCU6MxlNBvIx.jpg",
                Genre = "Sci-Fi",
                Year = 2014,
                Description = "Un gruppo di astronauti viaggia attraverso un wormhole alla ricerca di una nuova casa per l’umanità.",
                Plot = "In un futuro in cui la Terra è morente, l'ex pilota Cooper parte per una missione interstellare per trovare un nuovo pianeta abitabile, lasciando la sua famiglia con la promessa di tornare. Un viaggio tra spazio, tempo e amore."
            },
            new Movie
            {
                Title = "The Dark Knight",
                ImageUrl = "https://image.tmdb.org/t/p/w500/qJ2tW6WMUDux911r6m7haRef0WH.jpg",
                Genre = "Action",
                Year = 2008,
                Description = "Batman affronta il caos incarnato nel Joker in una lotta per Gotham.",
                Plot = "Il Joker, un criminale psicopatico, terrorizza Gotham City, spingendo Batman ai limiti morali del suo ruolo di eroe. Il cavaliere oscuro deve decidere fin dove spingersi per fermare l’anarchia e proteggere la città."
            },
            new Movie
            {
                Title = "Pulp Fiction",
                ImageUrl = "https://image.tmdb.org/t/p/w500/d5iIlFn5s0ImszYzBPb8JPIfbXD.jpg",
                Genre = "Crime",
                Year = 1994,
                Description = "Crimine, ironia e dialoghi taglienti in storie intrecciate a Los Angeles.",
                Plot = "Le vite di gangster, pugili, criminali e una coppia di rapinatori si intrecciano in una serie di eventi imprevedibili. Tarantino mescola violenza, humour e riflessioni morali in un capolavoro narrativo non lineare."
            },
            new Movie
            {
                Title = "Fight Club",
                ImageUrl = "https://image.tmdb.org/t/p/w500/pB8BM7pdSp6B6Ih7QZ4DrQ3PmJK.jpg",
                Genre = "Drama",
                Year = 1999,
                Description = "Un impiegato insoddisfatto trova sollievo fondando un club di lotta segreto.",
                Plot = "Un uomo in crisi esistenziale incontra Tyler Durden, un carismatico fabbricante di sapone. Insieme fondano il Fight Club, dove gli uomini si liberano dalle convenzioni sociali, fino a quando la follia prende il sopravvento."
            },
            new Movie
            {
                Title = "Forrest Gump",
                ImageUrl = "https://image.tmdb.org/t/p/w500/arw2vcBveWOVZr6pxd9XTd1TdQa.jpg",
                Genre = "Drama",
                Year = 1994,
                Description = "La storia di un uomo semplice con un cuore straordinario.",
                Plot = "Forrest Gump, un uomo dal basso quoziente intellettivo ma dal grande cuore, attraversa decenni di storia americana vivendo eventi epici e dimostrando che la bontà e l’amore possono cambiare il mondo."
            },
            new Movie
            {
                Title = "The Shawshank Redemption",
                ImageUrl = "https://image.tmdb.org/t/p/w500/q6y0Go1tsGEsmtFryDOJo3dEmqu.jpg",
                Genre = "Drama",
                Year = 1994,
                Description = "Due uomini in prigione trovano la libertà attraverso la speranza.",
                Plot = "Andy Dufresne, ingiustamente condannato per omicidio, trova amicizia e redenzione nella prigione di Shawshank. Con pazienza e ingegno, prepara per anni la fuga che gli ridarà la libertà."
            }
        });
        db.SaveChanges();
    }
}

// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

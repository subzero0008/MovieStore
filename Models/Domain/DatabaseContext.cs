using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.Domain;
using Npgsql.EntityFrameworkCore.PostgreSQL;


public class DatabaseContext : IdentityDbContext<ApplicationUser>
{
    private readonly string _connectionString;

    // Използване на инжектиране на зависимости
    public DatabaseContext(DbContextOptions<DatabaseContext> options, IConfiguration configuration) : base(options)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public DbSet<Genre> Genre { get; set; }
    public DbSet<MovieGenre> MovieGenre { get; set; }
    public DbSet<Movie> Movie { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}

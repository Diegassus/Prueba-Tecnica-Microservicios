using Microsoft.EntityFrameworkCore;
using MicroserviceMovies.Models;

namespace MicroserviceMovies.Database;

public class AppDbContext : DbContext
{
  public DbSet<Movie> Movies { get; set; }

  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
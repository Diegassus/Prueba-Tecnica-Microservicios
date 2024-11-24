using Microsoft.EntityFrameworkCore;
using MicroservicePersonas.Models;

namespace MicroservicePersonas.Database;

public class AppDbContext : DbContext
{
  public DbSet<Persona> Personas { get; set; }
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
using Microsoft.EntityFrameworkCore;
using RESTAPI.Models;

namespace RESTAPI.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Record> Records { get; set; }
    public DbSet<Currency> Currencies { get; set; }
}
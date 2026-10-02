using Microsoft.EntityFrameworkCore;
using PortfolioApi.Models;

namespace PortfolioApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Portfolio> Portfolios { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}
using Microsoft.EntityFrameworkCore;
using SafeScribe.Models;

namespace SafeScribe.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Note> Notes => Set<Note>();
    }
}

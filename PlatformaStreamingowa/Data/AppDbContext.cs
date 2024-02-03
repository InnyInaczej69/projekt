using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CustomIdentity.Models;

namespace CustomIdentity.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Film> Filmy { get; set; }
        public DbSet<Kategoria> Kategorie { get; set; }
        public DbSet<Ocena> Oceny { get; set; }
        public DbSet<Comment> Comments { get; set; } 

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.models
    {
    public class ApplicationContext : IdentityDbContext<ApplicationUser>
        {
        public ApplicationContext(DbContextOptions options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Loan> Loans { get; set; }

        }
    }

using Building_Minimal_APIs_With_ASPnet_Core_And_EF_Core_Movies_App.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_Minimal_APIs_With_ASPnet_Core_And_EF_Core_Movies_App
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Genre>()
                .Property(p => p.Name )
                .HasMaxLength(50);
        
        }

        public DbSet<Genre> Genres { get; set; } 
    }
}

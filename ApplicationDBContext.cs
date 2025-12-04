using Building_MinimalAPIsMoviesApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace Building_MinimalAPIsMoviesApp
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
                .Property(p => p.Name)
                .HasMaxLength(150);

            modelBuilder
                .Entity<Actor>()
                .Property(p => p.Name)
                .HasMaxLength(150);
            modelBuilder
                .Entity<Actor>()
                .Property(p => p.Picture)
                .IsUnicode(false);

            modelBuilder
                .Entity<Movie>()
                .Property(m => m.Title)
                .HasMaxLength(250);
            modelBuilder
                .Entity<Movie>()
                .Property(m => m.Poster)
                .IsUnicode();

            modelBuilder
                .Entity<GenreMovie>()
                .HasKey(gm => new { gm.MovieId, gm.GenreId});

        }

        public DbSet<Genre> Genres { get; set; } 
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<GenreMovie> GenresMovies { get; set; }
    }
}

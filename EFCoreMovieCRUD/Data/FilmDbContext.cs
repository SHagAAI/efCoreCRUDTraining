using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreMovieCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreMovieCRUD.Data
{
    public class FilmDbContext : DbContext
    {
        // The tables in your database
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Director> Directors => Set<Director>();


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Only configure if not already configured (allows for external configuration)
            if (!optionsBuilder.IsConfigured)
            {

                optionsBuilder.UseSqlite("Data Source=FilmDatabase.db");



                // Log detailed information for learning purposes - commented out for cleaner console output
                // optionsBuilder.LogTo(Console.WriteLine);
            }


        }

        /// <summary>
        /// OnModelCreating is where we configure our model using Fluent API
        /// This gives us more control than Data Annotations alone
        /// Think of it as fine-tuning how EF maps our classes to database tables
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>(genreEntity =>
            {
                // Ensure genre name are unique
                genreEntity.HasIndex(gnr => gnr.Name).IsUnique();
            });


            modelBuilder.Entity<Movie>(movieEntity =>
            {
                // Many to one relationship
                // between many Movie to one director
                movieEntity.HasOne(mE => mE.Director)
                .WithMany(d => d.Movies)
                .HasForeignKey(m=>m.DirectorId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure many-to-many relationship between Movie and Genre
            modelBuilder.Entity<Movie>()
            .HasMany(mov => mov.Genres)
            .WithMany(gnr => gnr.Movies)
            .UsingEntity<Dictionary<string, object>>(
                "MovieGenre", // Join table entity name
                j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId"),
                j => j.HasOne<Movie>().WithMany().HasForeignKey("MovieId"),
                j =>
                {
                    j.HasKey("MovieId", "GenreId"); // Composite primary key ????
                    j.ToTable("MovieGenres"); // Join table name in database
                }
            );

            /// composite primary key
            /// 
            /// 
        }

    }
}
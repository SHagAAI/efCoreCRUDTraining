using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreMovieCRUD.Data;
using EFCoreMovieCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreMovieCRUD.Services
{
    public class MovieService
    {
        private readonly FilmDbContext _context;

        public MovieService(FilmDbContext context)
        {
            _context = context;
        }

        // add new movie
        public async Task<Movie> CreateMovieAsync(Movie mov)
        {
            _context.Movies.Add(mov);

            await _context.SaveChangesAsync();
            Console.WriteLine($"Added movie {mov.Title}");
            return mov;

        }

        // update movie
        public async Task<Movie?> UpdateMovieAsync(int id, Movie updateMovie)
        {

            var movieExist = await _context.Movies.FindAsync(id);

            if (movieExist == null)
            {
                return null;
            }

            movieExist.Title = updateMovie.Title;
            movieExist.ReleaseYear = updateMovie.ReleaseYear;
            movieExist.Genres = updateMovie.Genres;
            movieExist.DirectorId = updateMovie.DirectorId;
            movieExist.Director = updateMovie.Director;

            await _context.SaveChangesAsync();
            return movieExist;

            // var movieExist = await _context.Movies.FindAsync(mov.Id);

            // if (movieExist == null)
            // {
            //     return false;
            // }

            // _context.Entry(movieExist).CurrentValues.SetValues(mov);

            // await _context.SaveChangesAsync();
            // return true;

        }

        // delete movie
        public async Task<bool> DeleteMovieAsync(int id)
        {
            var movieExist = await _context.Movies.FindAsync(id);

            if (movieExist == null)
            {
                return false;
            }

            _context.Movies.Remove(movieExist);
            await _context.SaveChangesAsync();
            return true;
        }


        // GET        
        // ALL movie
        public async Task<List<Movie>> GetAllMovieAsync()
        {
            return await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .ToListAsync();
        }

        // movie by ID
        public async Task<Movie?> GetMovieByIdAsync(int id)
        {
            return await _context.Movies
            .Include(m => m.Director)
            .Include(m => m.Genres)
            .FirstOrDefaultAsync(m => m.Id == id);
        }

        // GET MOVIE BY GENRE
        public async Task<List<Movie>> GetMovieByGenreAsync(int genreId)
        {
            return await _context.Movies
                .Include(m => m.Director)
                .Include(m => m.Genres)
                .Where(m => m.Genres.Any(g => g.Id == genreId))
                .ToListAsync();
        }

        public async Task<List<Movie>> GetMovieByDirectorAsync(int directorId)
        {
            return await _context.Movies
            .Include(m => m.Genres)
            .Where(m=>m.DirectorId == directorId)
            .ToListAsync();
        }

    }
}
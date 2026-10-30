using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreMovieCRUD.Data;
using EFCoreMovieCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreMovieCRUD.Services
{
    public class GenreService
    {
        private readonly FilmDbContext _context;

        public GenreService(FilmDbContext context)
        {
            _context = context;
        }

        // CREATE NEW GENRE
        public async Task<Genre> CreateGenreAsync(Genre gnr)
        {
            var gnrExist = await _context.Genres.FirstOrDefaultAsync(g => g.Name == gnr.Name);

            if (gnrExist != null)
            {
                throw new InvalidOperationException("Genre name already exists, please choose other name");
            }

            _context.Genres.Add(gnr);
            await _context.SaveChangesAsync();
            return gnr;

        }

        public async Task<Genre?> UpdateGenreAsync(int id, Genre updateGenre)
        {
            var gnrExist = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

            if (gnrExist == null)
            {
                return null;
            }

            gnrExist.Name = updateGenre.Name;
            gnrExist.Movies = updateGenre.Movies;

            await _context.SaveChangesAsync();
            return gnrExist;
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            var gnrExist = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);

            if (gnrExist == null)
            {
                return false;
            }

            _context.Genres.Remove(gnrExist);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Genre>> GetAllGenreAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
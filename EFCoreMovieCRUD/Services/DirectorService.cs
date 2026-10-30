using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EFCoreMovieCRUD.Data;
using EFCoreMovieCRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreMovieCRUD.Services
{
    public class DirectorService
    {
        private readonly FilmDbContext _context;

        public DirectorService(FilmDbContext context)
        {
            _context = context;
        }

        public async Task<Director> CreateDirectorAsync(Director drctr)
        {
            var drctrExist = await _context.Directors.FirstOrDefaultAsync(d => d.Id == drctr.Id);

            if (drctrExist != null)
            {
                throw new InvalidOperationException("Genre name already exists, please choose other name");
            }

            Console.WriteLine($"added {drctr.Name}");

            _context.Directors.Add(drctr);
            await _context.SaveChangesAsync();
            return drctr;
        }
        public async Task<Director?> UpdateDirectorAsync(int id, Director updateDirector)
        {
            var drctrExist = await _context.Directors.FirstOrDefaultAsync(d => d.Id == updateDirector.Id);

            if (drctrExist == null)
            {
                return null;
            }

            drctrExist.Name = updateDirector.Name;
            drctrExist.Movies = updateDirector.Movies;

            await _context.SaveChangesAsync();
            return drctrExist;
        }
        public async Task<bool> DeleteDirectorAsync(int id)
        {
            var drctrExist = await _context.Directors.FindAsync(id);

            if (drctrExist == null)
            {
                return false;
            }

            _context.Directors.Remove(drctrExist);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Director>> GetAllDirectorAsync()
        {
            return await _context.Directors.ToListAsync();
        }
        public async Task<Director?> GetDirectorByIdAsync(int id)
        {
            return await _context.Directors
            .FirstOrDefaultAsync(d => d.Id == id);
        }

        
    }
}
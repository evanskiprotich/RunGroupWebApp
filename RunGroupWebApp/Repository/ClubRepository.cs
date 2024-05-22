using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class ClubRepository : IClubRepository
    {
        private readonly DataContext _context;

        public ClubRepository(DataContext context)
        {
            _context = context;
        }

        public bool AddClub(Club club)
        {
            _context.Add(club);
            return Save();
        }

        public bool DeleteClub(Club club)
        {
            _context.Remove(club);
            return Save();
        }

        public async Task<IEnumerable<Club>> GetAll()
        {
            var clubs = await _context.Clubs.ToListAsync();
            return clubs;
        }

        public async Task<Club> GetById(int id)
        {
            return await _context.Clubs.Include(i => i.Address).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Club> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Clubs.Include(i => i.Address).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Club>> GetClubByCity(string city)
        {
            return await _context.Clubs.Where(c => c.Address.City.Contains(city)).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateClub(Club club)
        {
            _context.Update(club);
            return Save();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class RaceRepository : IRaceRepository
    {
        private readonly DataContext _context;

        public RaceRepository(DataContext context)
        {
            _context = context;
        }
        public bool AddRace(Race race)
        {
            _context.Add(race);
            return Save();
        }

        public bool DeleteRace(Race race)
        {
            _context.Remove(race);
            return Save();
        }

        public async Task<IEnumerable<Race>> GetAll()
        {
            var races = await _context.Races.ToListAsync();
            return races;
        }

        public async Task<Race> GetById(int id)
        {
            return await _context.Races.Include(i => i.Address).FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Club> GetByIdAsyncNoTracking(int id)
        {
            return await _context.Clubs.Include(i => i.Address).AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<IEnumerable<Race>> GetAllRacesByCity(string city)
        {
            return await _context.Races.Where(c => c.Address.City.Contains(city)).ToListAsync();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateRace(Race race)
        {
            _context.Update(race);
            return Save();
        }
    }
}

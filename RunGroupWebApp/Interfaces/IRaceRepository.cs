using RunGroupWebApp.Models;

namespace RunGroupWebApp.Interfaces
{
    public interface IRaceRepository
    {
        Task<IEnumerable<Race>> GetAll();
        Task<Race> GetById(int id);
        Task<Club> GetByIdAsyncNoTracking(int id);
        Task<IEnumerable<Race>> GetAllRacesByCity(string city);
        bool AddRace(Race race);
        bool UpdateRace(Race race);
        bool DeleteRace(Race race);
        bool Save();
    }
}

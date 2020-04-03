using System.Collections.Generic;
using System.Threading.Tasks;
using TrackMyTime.Models;

namespace TrackMyTime.Repositories
{
    public interface IMyTimeRepo
    {
        Task<List<TimeModel>> GetTimesAsync(string userId, string timeGroup);

        Task AddTimeAsync(TimeModel item);


        Task<List<TimeGroupModel>> GetTimeGroupsAsync(string userId);

        Task AddTimeGroupsAsync(TimeGroupModel group);
    }
}
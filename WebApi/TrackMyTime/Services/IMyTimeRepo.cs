using System.Collections.Generic;
using System.Threading.Tasks;
using TrackMyTime.Models;

namespace TrackMyTime.Services
{
    public interface IMyTimeRepo
    {
        List<TimeModel> GetItems(string userId, string timeGroup);
        Task<TimeModel> GetItemAsync(string id, string userId);
        Task AddItemAsync(TimeModel item);
    }
}
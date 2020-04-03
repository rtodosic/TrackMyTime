using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using TrackMyTime.Models;

namespace TrackMyTime.Repositories
{
    [ExcludeFromCodeCoverage]
    public class MyTimeRepo: IMyTimeRepo
    {
        private readonly Container _timeContainer;
        private readonly Container _timeGroupContainer;

        public MyTimeRepo(Container timeContainer, Container timeGroupContainer)
        {
            _timeContainer = timeContainer;
            _timeGroupContainer = timeGroupContainer;
        }

        public async Task AddTimeAsync(TimeModel item)
        {
            await _timeContainer.CreateItemAsync(item, new PartitionKey(item?.UserId)).ConfigureAwait(true);
        }

        public async Task<List<TimeModel>> GetTimesAsync(string userId, string timeGroup)
        {
            var query = new QueryDefinition("SELECT * FROM Time t WHERE t.userId = @UserId AND t.timeGroup = @TimeGroup ORDER BY t.start DESC")
               .WithParameter("@UserId", userId)
               .WithParameter("@TimeGroup", timeGroup);

            var iterator = _timeContainer.GetItemQueryIterator<TimeModel>(query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(userId)
                });

            var times = new List<TimeModel>();
            while (iterator.HasMoreResults)
            {
                var documents = await iterator.ReadNextAsync().ConfigureAwait(true);
                foreach (var time in documents)
                    times.Add(time);
            }

            return times;
        }

        public async Task<List<TimeGroupModel>> GetTimeGroupsAsync(string userId)
        {
            var query = new QueryDefinition("SELECT * FROM TimeGroup g WHERE g.userId = @UserId ORDER BY g.name")
                .WithParameter("@UserId", userId);

            var iterator = _timeGroupContainer.GetItemQueryIterator<TimeGroupModel>(query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(userId)
                });

            var timeGroups = new List<TimeGroupModel>();
            while (iterator.HasMoreResults)
            {
                var documents = await iterator.ReadNextAsync().ConfigureAwait(true);
                foreach (var timeGroup in documents)
                    timeGroups.Add(timeGroup);
            }

            return timeGroups;
        }


        public async Task AddTimeGroupsAsync(TimeGroupModel group)
        {
            await _timeGroupContainer.CreateItemAsync(group, new PartitionKey(group?.UserId)).ConfigureAwait(true);
        }
    }
}

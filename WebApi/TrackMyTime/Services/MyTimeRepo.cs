using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using TrackMyTime.Models;

namespace TrackMyTime.Services
{
    [ExcludeFromCodeCoverage]
    public class MyTimeRepo: IMyTimeRepo
    {
        private Container _container;

        public MyTimeRepo(Container container)
        {
            _container = container;
        }

        public async Task AddItemAsync(TimeModel item)
        {
            await _container.CreateItemAsync(item, new PartitionKey(item?.UserId)).ConfigureAwait(false);
        }

        public async Task<TimeModel> GetItemAsync(string id, string userId)
        {
            try
            {
                ItemResponse<TimeModel> response = await _container.ReadItemAsync<TimeModel>(id, new PartitionKey(userId)).ConfigureAwait(false);
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public List<TimeModel> GetItems(string userId, string timeGroup)
        {
            var query = from t in _container.GetItemLinqQueryable<TimeModel>(allowSynchronousQueryExecution: true)
                        where t.UserId == userId && t.TimeGroup == timeGroup
                        orderby t.Start descending
                        select t;

            return query.ToList();
        }
    }
}

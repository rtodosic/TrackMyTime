using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackMyTime.Models;
using TrackMyTime.Repositories;

namespace TrackMyTime.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TimeGroupController : Controller
    {
        private readonly IMyTimeRepo _myTimeRepo;

        public TimeGroupController(IMyTimeRepo myTimeRepo)
        {
            _myTimeRepo = myTimeRepo;
        }

        [HttpGet]
        public async Task<IEnumerable<TimeGroupModel>> GetAsync()
        {
            var groups = await _myTimeRepo.GetTimeGroupsAsync("123").ConfigureAwait(true);
            return groups;
        }
    }
}
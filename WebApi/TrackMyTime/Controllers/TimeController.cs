using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrackMyTime.Models;
using TrackMyTime.Repositories;

// https://www.hanselman.com/blog/ASPNETCoreRESTfulWebAPIVersioningMadeEasy.aspx
// https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnet-application

namespace TrackMyTime.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TimeController : Controller
    {
        private readonly IMyTimeRepo _myTimeRepo;
        private readonly ILogger<TimeController> _logger;

        public TimeController(IMyTimeRepo myTimeRepo, ILogger<TimeController> logger)
        {
            _myTimeRepo = myTimeRepo;
            _logger = logger;
        }


        [HttpGet(Name = "GetByGroup")]
        public async Task<IEnumerable<TimeModel>> GetByGroupAsync(string timeGroup)
        {
            var myTimes = await _myTimeRepo.GetTimesAsync("123", timeGroup).ConfigureAwait(true);
            return myTimes;
        }

        [HttpPost]
        public ActionResult<TimeModel> Post([Bind("Id,UserId,TimeGroup,Start,End,Notes")]TimeModel model)
        {
            if (string.IsNullOrWhiteSpace(model?.UserId))
                return BadRequest("JSON userId property must be set.");

            if (string.IsNullOrWhiteSpace(model.TimeGroup))
                return BadRequest("JSON timeGroup property must be set.");

            if (model.Start == null)
                return BadRequest("JSON start property must be set.");

            if (model.End == null)
                return BadRequest("JSON end property must be set.");

            try
            {
                model.Id = Guid.NewGuid().ToString();
                _myTimeRepo.AddTimeAsync(model);

                try
                {
                    _myTimeRepo.AddTimeGroupsAsync(new TimeGroupModel()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = model.TimeGroup,
                        UserId = model.UserId
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unable to post time group");
                }

                return CreatedAtRoute(new { id = model.TimeGroup }, model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to post time");
                return StatusCode(StatusCodes.Status500InternalServerError, "Data persist failure");
            }
        }
    }
}
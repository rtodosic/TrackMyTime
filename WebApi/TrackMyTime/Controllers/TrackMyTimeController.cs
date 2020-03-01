using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using TrackMyTime.Models;
using TrackMyTime.Services;

// https://www.hanselman.com/blog/ASPNETCoreRESTfulWebAPIVersioningMadeEasy.aspx
// https://docs.microsoft.com/en-us/azure/cosmos-db/sql-api-dotnet-application

namespace TrackMyTime.Controllers
{
    [Produces("application/json")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class TrackMyTimeController : Controller
    {
        private readonly IMyTimeRepo _myTimeRepo;

        public TrackMyTimeController(IMyTimeRepo myTimeRepo)
        {
            _myTimeRepo = myTimeRepo;
        }


        [HttpGet(Name = "GetByGroup")]
        public IEnumerable<TimeModel> GetByGroup(string timeGroup)
        {
            var myTimes = _myTimeRepo.GetItems("123", timeGroup);
            return myTimes;
        }

        [HttpPost]
        public ActionResult<TimeModel> Post([Bind("Id,UserId,TimeGroup,Start,End,Notes")]TimeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId))
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
                _myTimeRepo.AddItemAsync(model);
                return Created("api/v1/trackmytime/?timeGroup=" + Uri.EscapeDataString(model.TimeGroup) , model);
            }
            catch
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Data persist failure");
            }
        }
    }
}
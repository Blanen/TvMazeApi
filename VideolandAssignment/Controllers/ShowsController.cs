using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideolandAssignment.DTOModels;
using VideolandAssignment.Managers;

namespace VideolandAssignment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        // POST api/values
        [HttpPost("updateDb")]
        public void Post()
        {
            var manager = new VideolandAssignmentManager();
            manager.UpdateShows();
        }

        [HttpGet]
        public async Task<List<ShowListDto>> Index([FromQuery(Name = "page")]int page = 0)
        {
            var manager = new VideolandAssignmentManager();
            return await manager.GetShowListByPage(page);
        }

    }
}

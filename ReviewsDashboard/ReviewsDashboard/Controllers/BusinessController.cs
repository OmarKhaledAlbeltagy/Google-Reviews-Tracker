using FluentScheduler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewsDashboard.Models;
using ReviewsDashboard.Repos;

namespace ReviewsDashboard.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessRep rep;

        public BusinessController(IBusinessRep rep)
        {
            this.rep = rep;
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public async Task<IActionResult> AddNewBusiness(AddBusinessTrack obj)
        {
            return Ok(rep.AddNewBusiness(obj).Result);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteBusiness(int id)
        {
            return Ok(rep.DeleteBusiness(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult ResumeTracking(int id)
        {
            return Ok(rep.ResumeTracking(id).Result);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult PauseTracking(int id)
        {
            return Ok(rep.PauseTracking(id));
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult StartAllTracks()
        {
            return Ok(rep.StartTrackAll().Result);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllTracks()
        {
            return Ok(JobManager.AllSchedules);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetInHistory()
        {
            return Ok(rep.GetInHistory());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetInProgress()
        {
            return Ok(rep.GetInProgress());
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetPaused()
        {
            return Ok(rep.GetPaused());
        }
    }
}

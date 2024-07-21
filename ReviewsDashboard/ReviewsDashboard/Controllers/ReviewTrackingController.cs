using FluentScheduler;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ReviewsDashboard.Context;
using ReviewsDashboard.Models;
using ReviewsDashboard.Repos;
using System.ComponentModel.DataAnnotations;

namespace ReviewsDashboard.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class ReviewTrackingController : ControllerBase
    {
        private readonly IReviewTrackingRep rep;

        public ReviewTrackingController(IReviewTrackingRep rep)
        {
            this.rep = rep;
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult DeleteReview(int id)
        {
            return Ok(rep.DeleteReview(id));
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult Retrack(int id)
        {
            return Ok(rep.Retrack(id).Result);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult StartAllTracks()
        {
            return Ok(rep.StartTrackAll().Result);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult StopAllTracks()
        {
            var x = JobManager.AllSchedules;
            foreach (var item in x)
            {
                JobManager.GetSchedule(item.Name).Disable();
            }
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult GetAllTracks()
        {
            return Ok(JobManager.AllSchedules);
        }

        [Route("[controller]/[Action]/{id}")]
        [HttpGet]
        public IActionResult StopTracking(int id)
        {
            return Ok(rep.StopTracking(id));
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
        [HttpPost]
        public IActionResult AddNewTracker(AddReviewTrack obj)
        {
            return Ok(rep.AddNewTracker(obj).Result);
        }


    }
}

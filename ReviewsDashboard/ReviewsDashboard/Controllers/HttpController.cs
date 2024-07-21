using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReviewsDashboard.Models;
using ReviewsDashboard.Repos;
using System.Net.Mail;
using System.Net;
using ReviewsDashboard.Context;
using ReviewsDashboard.Privilage;
using Microsoft.AspNetCore.Identity;

namespace ReviewsDashboard.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class HttpController : ControllerBase
    {
        private readonly IHttpRep rep;
        private readonly UserManager<ExtendIdentityUser> userManager;

        public HttpController(IHttpRep rep, UserManager<ExtendIdentityUser> userManager)
        {
            this.rep = rep;
            this.userManager = userManager;
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> Test()
        {
            DateTime d = DateTime.Now.AddDays(-7).AddHours(-12);
            long x = new DateTimeOffset(d).ToUnixTimeSeconds();
            return Ok(x);
        }


        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> ChangeEmail()
        {
            ExtendIdentityUser user = await userManager.FindByIdAsync("aaf561a3-6d3f-421a-802a-4c5d40062ce9");

            var x = userManager.SetEmailAsync(user, "businesstracker2024@gmail.com");
            var y = userManager.SetUserNameAsync(user, "businesstracker2024@gmail.com");
            if (x.IsCompletedSuccessfully && y.IsCompletedSuccessfully)
            {
                return Ok(true);
            }

            return Ok(false);
        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetBusiness(GetBusinessModel obj)
        {
            string link = rep.GetRedirectingUrl(obj.Link);

            obj.Link = link;

            string res = rep.GetBusiness(obj).Result;

            return new ContentResult()
            {
                Content = res,
                ContentType = "application/json"
            };
        }



        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult EmailTest()
        {
            MailMessage m = new MailMessage();
            m.To.Add("mohamedtalaat.gmt@gmail.com");
            m.To.Add("omar.elbeltagy.1993@gmail.com");
            m.Subject = "Review is still exist";
            m.From = new MailAddress("businesstracker@businesstracker.site");
            m.Sender = new MailAddress("businesstracker@businesstracker.site");
            m.IsBodyHtml = true;
            m.Body = "Your Review is still exist";
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
            smtp.EnableSsl = false;
            smtp.Credentials = new NetworkCredential("businesstracker@businesstracker.site", "a6r^YQ@80:5R");
            smtp.Send(m);
            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetRedirectingUrl(LinkModel obj)
        {
            return Ok(rep.GetRedirectingUrl(obj.Link));
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public IActionResult GetReviews(GetReviewModel obj)
        {
            string link = rep.GetRedirectingUrl(obj.Link);

            obj.Link = link;

            string res =  rep.GetReviews(obj).Result;

            return new ContentResult()
            {
                Content = res,
                ContentType = "application/json"
            };
        }
    }
}

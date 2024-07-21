using FluentScheduler;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ReviewsDashboard.Context;
using ReviewsDashboard.Entities;
using ReviewsDashboard.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Administration;


namespace ReviewsDashboard.Repos
{
    public class ReviewTrackingRep: IReviewTrackingRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly ICheckRep chk;
        public ReviewTrackingRep(DbContainer db, ITimeRep ti, ICheckRep chk)
        {
            this.db = db;
            this.ti = ti;
            this.chk = chk;
        }

        public async Task<bool> AddNewTracker(AddReviewTrack obj)
        {

            DateTime now = ti.GetCurrentTime();
            try
            {
                Review r = new Review();
                r.StartTracking = now;
                r.AddingDateTime = now;
                r.LocationUrl = obj.LocationUrl;
                r.ReviewText = obj.ReviewText;
                r.ReviewDateTime = obj.ReviewDateTime;
                r.Rating = obj.Rating;
                r.ImageUrl = obj.ImageUrl;
                r.ReviewGoogleId = obj.ReviewGoogleId;
                r.PlaceName = obj.PlaceName;
                r.ReviewAuthor = obj.ReviewAuthor;
                r.Status = "In Progress";
                db.review.Add(r);
                db.SaveChanges();

           
                Registry registry = new Registry();
                registry.Schedule(() => chk.CheckReview(r.Id)).WithName("SixHours" + r.Id).ToRunEvery(3).Days();
                registry.Schedule(() => chk.CheckReview(r.Id)).WithName("OneDay" + r.Id).ToRunEvery(1).Weeks().DelayFor(30).Days();
                JobManager.Initialize(registry);

                string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/Mail.html");
                body = body.Replace("[ReviewText]", r.ReviewText).Replace("[body]", "Your review for "+r.PlaceName+" is now being tracked!<br>").Replace("[ReviewTime]", r.ReviewDateTime).Replace("[RemainingTime]", (7 - ((now - r.StartTracking)).Days).ToString()).Replace("[LocationUrl]",r.LocationUrl).Replace("[Rating]",r.Rating);
                MailMessage m = new MailMessage();
                m.To.Add(db.Users.First().Email);
                var mails = db.email.Select(a => a.Email).ToList();

                foreach (var item in mails)
                {
                    m.To.Add(item);
                }

                m.Subject = r.PlaceName + " Review Tracking Report";
                m.From = new MailAddress("businesstracker@businesstracker.site");
                m.Sender = new MailAddress("businesstracker@businesstracker.site");
                m.IsBodyHtml = true;
                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential("businesstracker@businesstracker.site", "a6r^YQ@80:5R");
                smtp.Send(m);
                return true;
            }


             catch (Exception ex)
            {
                var message = ex.Message;
                var inner = ex.InnerException.Message;
                Properties p = new Properties();
                p.Property = now.AddHours(8).ToString("dd MM yy - hh:mm tt");
                p.Value = message + " -- " + inner;
                db.properties.Add(p);
                db.SaveChanges();
                return false;
            }
        }

        public bool DeleteReview(int id)
        {
            Review r = db.review.Find(id);
            db.review.Remove(r);
            db.SaveChanges();
            return true;
        }

        public List<InProgressModel> GetInHistory()
        {
            DateTime now = ti.GetCurrentTime();
   
            List<Review> rev = db.review.Where(a=>a.Status == "Canceled" || a.Status == "Completed" || a.Status == "Removed").ToList();
            List<InProgressModel> res = new List<InProgressModel>();
            foreach (var item in rev)
            {
                InProgressModel obj = new InProgressModel();
                obj.Id = item.Id;
                obj.LocationUrl = item.LocationUrl;
                obj.ReviewText = item.ReviewText;
                obj.PlaceName = item.PlaceName;
                obj.ReviewAuthor = item.ReviewAuthor;
                obj.ReviewDateTime = item.ReviewDateTime;
                obj.ReviewGoogleId = item.ReviewGoogleId;
                obj.ImageUrl = item.ImageUrl;
                obj.Rating = item.Rating;
                obj.AddingDateTime = item.AddingDateTime;
                obj.StartTracking = item.StartTracking;
                obj.DaysRemaining = 0;
                obj.Status = item.Status;
                res.Add(obj);
            }

            return res;
        }

        public List<InProgressModel> GetInProgress()
        {
            DateTime now = ti.GetCurrentTime();

            List<Review> rev = db.review.Where(a => a.Status == "In Progress").ToList();
            List<InProgressModel> res = new List<InProgressModel>();
            foreach (var item in rev)
            {
                InProgressModel obj = new InProgressModel();
                obj.Id = item.Id;
                obj.LocationUrl = item.LocationUrl;
                obj.ReviewText = item.ReviewText;
                obj.PlaceName = item.PlaceName;
                obj.ReviewAuthor = item.ReviewAuthor;
                obj.ReviewDateTime = item.ReviewDateTime;
                obj.ReviewGoogleId = item.ReviewGoogleId;
                obj.ImageUrl = item.ImageUrl;
                obj.Rating = item.Rating;
                obj.AddingDateTime = item.AddingDateTime;
                obj.StartTracking = item.StartTracking;
                obj.DaysRemaining = (60 - ((now - item.StartTracking)).Days);
                res.Add(obj);
            }
            return res;
        }

        public async Task<bool> Retrack(int id)
        {

            DateTime now = ti.GetCurrentTime();
            try
            {

                Review r = db.review.Find(id);
                r.StartTracking = now;
                r.Status = "In Progress";
                db.SaveChanges();


                Registry registry = new Registry();
                registry.Schedule(() => chk.CheckReview(r.Id)).WithName("SixHours" + r.Id).ToRunEvery(3).Days();
                registry.Schedule(() => chk.CheckReview(r.Id)).WithName("OneDay" + r.Id).ToRunEvery(1).Weeks().DelayFor(30).Days();
                JobManager.Initialize(registry);

                string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/Mail.html");
                body = body.Replace("[ReviewText]", r.ReviewText).Replace("[body]", "Your review for " + r.PlaceName + " is now being tracked again!<br>").Replace("[ReviewTime]", r.ReviewDateTime).Replace("[RemainingTime]", (7 - ((now - r.StartTracking)).Days).ToString()).Replace("[LocationUrl]", r.LocationUrl).Replace("[Rating]", r.Rating);
                MailMessage m = new MailMessage();
                m.To.Add(db.Users.First().Email);
                var mails = db.email.Select(a => a.Email).ToList();
                foreach (var item in mails)
                {
                    m.To.Add(item);
                }

                m.Subject = r.PlaceName + " Review Tracking Report";
                m.From = new MailAddress("businesstracker@businesstracker.site");
                m.Sender = new MailAddress("businesstracker@businesstracker.site");
                m.IsBodyHtml = true;
                m.Body = body;
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
                smtp.EnableSsl = false;
                smtp.Credentials = new NetworkCredential("businesstracker@businesstracker.site", "a6r^YQ@80:5R");
                smtp.Send(m);
                return true;
            }


            catch (Exception ex)
            {
                var message = ex.Message;
                var inner = ex.InnerException.Message;
                Properties p = new Properties();
                p.Property = now.AddHours(8).ToString("dd MM yy - hh:mm tt");
                p.Value = message + " -- " + inner;
                db.properties.Add(p);
                db.SaveChanges();
                return false;
            }
        }

        public async Task<bool> StartTrackAll()
        {
            List<Review> reviews = db.review.Where(a=>a.Status == "In Progress").ToList();
            List<Business> business = db.business.Where(a=>a.Status == "In Progress").ToList();

            foreach (var item in reviews)
            {
                Registry registry = new Registry();
                //3 Days
                registry.Schedule(() => chk.CheckReview(item.Id)).WithName("SixHours" + item.Id).ToRunEvery(3).Days();
                registry.Schedule(() => chk.CheckReview(item.Id)).WithName("OneDay" + item.Id).ToRunEvery(1).Weeks().DelayFor(30).Days();
                JobManager.Initialize(registry);

            }
            foreach (var item in business)
            {
                Registry registry = new Registry();
                registry.Schedule(() => chk.CheckBusiness(item.Id)).WithName("Weekly" + item.Id).ToRunEvery(1).Weeks();
                JobManager.Initialize(registry);
            }
            return true;
        }

        public bool StopTracking(int id)
        {
            DateTime now = ti.GetCurrentTime();
            Review r = db.review.Find(id);



            var x = JobManager.GetSchedule("SixHours" + r.Id);
            var y = JobManager.GetSchedule("OneDay" + r.Id);

            if (x != null)
            {
                x.Disable();

            }
            if (y != null)
            {
                y.Disable();

            }
            r.Status = "Canceled";
            db.SaveChanges();


            string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/Mail.html");
            body = body.Replace("[ReviewText]", r.ReviewText).Replace("[body]", "Your tracking task for your review on "+r.PlaceName+" has been stopped as per your request.<br>").Replace("[ReviewTime]", r.ReviewDateTime).Replace("[RemainingTime]", (7 - ((now - r.StartTracking)).Days).ToString()).Replace("[LocationUrl]", r.LocationUrl).Replace("[Rating]", r.Rating);

            MailMessage m = new MailMessage();
            m.To.Add(db.Users.First().Email);

            var mails = db.email.Select(a=>a.Email).ToList();

            foreach (var item in mails)
            {
                m.To.Add(item);
            }

            m.Subject = r.PlaceName + " Review Tracking Report";
            m.From = new MailAddress("businesstracker@businesstracker.site");
            m.Sender = new MailAddress("businesstracker@businesstracker.site");
            m.IsBodyHtml = true;
            m.Body = body;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
            smtp.EnableSsl = false;
            smtp.Credentials = new NetworkCredential("businesstracker@businesstracker.site", "a6r^YQ@80:5R");
            smtp.Send(m);


            return true;
        }
    }
}

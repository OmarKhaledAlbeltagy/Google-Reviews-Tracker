using FluentScheduler;
using ReviewsDashboard.Context;
using ReviewsDashboard.Entities;
using ReviewsDashboard.Models;
using System.Net.Mail;
using System.Net;

namespace ReviewsDashboard.Repos
{
    public class BusinessRep: IBusinessRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        private readonly ICheckRep chk;

        public BusinessRep(DbContainer db, ITimeRep ti, ICheckRep chk)
        {
            this.db = db;
            this.ti = ti;
            this.chk = chk;
        }

        public async Task<bool> AddNewBusiness(AddBusinessTrack obj)
        {
            DateTime now = ti.GetCurrentTime();
            try
            {
                Business b = new Business();
                b.StartTracking = now;
                b.AddingDateTime = now;
                b.LocationLink = obj.LocationLink;
                b.NumberOfReviews = obj.NumberOfReviews;
                b.BusinessName = obj.BusinessName;
                b.Rating = obj.Rating;
                b.ImgUrl = obj.ImgUrl;
                b.Status = "In Progress";
                db.business.Add(b);
                db.SaveChanges();

                Registry registry = new Registry();
                registry.Schedule(() => chk.CheckBusiness(b.Id)).WithName("Weekly" + b.Id).ToRunEvery(1).Weeks();
                JobManager.Initialize(registry);


                var x =  chk.CheckBusiness(b.Id);

                string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/BusinessMail.html");
                body = body.Replace("[BusinessName]", b.BusinessName).Replace("[body]", "The business is now being tracked!<br>").Replace("[RemainingTime]", (365 - ((now - b.StartTracking)).Days).ToString()).Replace("[LocationUrl]", b.LocationLink).Replace("[Rating]", b.Rating.ToString());
                MailMessage m = new MailMessage();
                m.To.Add(db.Users.First().Email);
                var mails = db.email.Select(a => a.Email).ToList();

                foreach (var item in mails)
                {
                    m.To.Add(item);
                }

                m.Subject = "Your Business " + b.BusinessName + " Tracking Report";
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

        public bool DeleteBusiness(int id)
        {
            Business b = db.business.Find(id);
            db.business.Remove(b);
            db.SaveChanges();
            return true;
        }

        public List<BusinessModel> GetInHistory()
        {
            DateTime now = ti.GetCurrentTime();

            List<Business> bus = db.business.Where(a => a.Status == "Completed").ToList();
            List<BusinessModel> res = new List<BusinessModel>();
            foreach (var item in bus)
            {
                BusinessModel obj = new BusinessModel();
                obj.Id = item.Id;
                obj.LocationLink = item.LocationLink;
                obj.BusinessName = item.BusinessName;
                obj.NumberOfReviews = item.NumberOfReviews;
                obj.DaysRemaining = 0;
                obj.ImgUrl = item.ImgUrl;
                obj.Status = item.Status;
                obj.LastWeek = 0;
                obj.LastMonth = 0; 
                obj.Rating = item.Rating;
                res.Add(obj);
            }

            return res;
        }

        public List<BusinessModel> GetInProgress()
        {
            DateTime now = ti.GetCurrentTime();
            List<Business> bus = db.business.Where(a => a.Status == "In Progress").ToList();
            List<BusinessModel> res = new List<BusinessModel>();
            foreach (var item in bus)
            {
                BusinessModel obj = new BusinessModel();
                obj.Id = item.Id;
                obj.LocationLink = item.LocationLink;
                obj.BusinessName = item.BusinessName;
                obj.NumberOfReviews = item.NumberOfReviews;
                obj.DaysRemaining = (365 - ((now - item.StartTracking)).Days);
                obj.ImgUrl = item.ImgUrl;
                obj.Status = item.Status;
                obj.LastWeek = item.Week1;
                obj.LastMonth = item.Week1 + item.Week2 + item.Week3 + item.Week4;
                obj.Rating = item.Rating;
                res.Add(obj);
            }
            return res;
        }

        public List<BusinessModel> GetPaused()
        {
            DateTime now = ti.GetCurrentTime();
            List<Business> bus = db.business.Where(a => a.Status == "Paused").ToList();
            List<BusinessModel> res = new List<BusinessModel>();
            foreach (var item in bus)
            {
                BusinessModel obj = new BusinessModel();
                obj.Id = item.Id;
                obj.LocationLink = item.LocationLink;
                obj.BusinessName = item.BusinessName;
                obj.NumberOfReviews = item.NumberOfReviews;
                obj.DaysRemaining = (365 - ((now - item.StartTracking)).Days);
                obj.ImgUrl = item.ImgUrl;
                obj.Status = item.Status;
                obj.LastWeek = 0;
                obj.LastMonth = 0;
                obj.Rating = item.Rating;
                res.Add(obj);
            }
            return res;
        }

        public bool PauseTracking(int id)
        {
            DateTime now = ti.GetCurrentTime();
            Business b = db.business.Find(id);



            var x = JobManager.GetSchedule("Weekly" + b.Id);

            if (x != null)
            {
                x.Disable();
            }
        
            b.Status = "Paused";
            db.SaveChanges();


            string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/BusinessMail.html");

            body = body.Replace("[BusinessName]", b.BusinessName).Replace("[body]", "Your tracking task for your business " + b.BusinessName + " has been paused as per your request.<br>").Replace("[RemainingTime]", (365 - ((now - b.StartTracking)).Days).ToString()).Replace("[LocationUrl]", b.LocationLink).Replace("[Rating]", b.Rating.ToString());
            MailMessage m = new MailMessage();
            m.To.Add(db.Users.First().Email);

            var mails = db.email.Select(a => a.Email).ToList();

            foreach (var item in mails)
            {
                m.To.Add(item);
            }

            m.Subject = b.BusinessName + " Tracking Report";
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

        public async Task<bool> ResumeTracking(int id)
        {

            DateTime now = ti.GetCurrentTime();
            try
            {

                Business b = db.business.Find(id);

                if ((365 - (now - b.StartTracking).Days) < 7)
                {
                    b.Status = "Completed";
                    db.SaveChanges();
                    return true;
                }
                else
                {
                    b.Status = "In Progress";
                    db.SaveChanges();
                    Registry registry = new Registry();
                    registry.Schedule(() => chk.CheckBusiness(b.Id)).WithName("Weekly" + b.Id).ToRunEvery(1).Weeks();
                    JobManager.Initialize(registry);
                    chk.CheckBusiness(id);
                    string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/BusinessMail.html");
                    body = body.Replace("[BusinessName]", b.BusinessName).Replace("[body]", "Your tracking task for your business " + b.BusinessName + " has been resumed as per your request.<br>").Replace("[RemainingTime]", (365 - ((now - b.StartTracking)).Days).ToString()).Replace("[LocationUrl]", b.LocationLink).Replace("[Rating]", b.Rating.ToString());
                    MailMessage m = new MailMessage();
                    m.To.Add(db.Users.First().Email);
                    var mails = db.email.Select(a => a.Email).ToList();
                    foreach (var item in mails)
                    {
                        m.To.Add(item);
                    }

                    m.Subject = b.BusinessName + " Tracking Report";
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
            List<Business> business = db.business.ToList();
            foreach (var item in business)
            {
                Registry registry = new Registry();
                registry.Schedule(() => chk.CheckBusiness(item.Id)).WithName("Weekly" + item.Id).ToRunEvery(1).Weeks();
                JobManager.Initialize(registry);
            }
            return true;
        }
    }
}

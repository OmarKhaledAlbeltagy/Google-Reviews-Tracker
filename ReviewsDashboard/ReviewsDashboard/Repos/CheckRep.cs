using FluentScheduler;
using Newtonsoft.Json;
using ReviewsDashboard.Context;
using ReviewsDashboard.Entities;
using ReviewsDashboard.Models;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace ReviewsDashboard.Repos
{
    public class CheckRep : ICheckRep
    {

        private readonly ITimeRep ti;
        private readonly IDbContextFactory<DbContainer> DbFactory;
        public CheckRep(ITimeRep ti, IDbContextFactory<DbContainer> DbFactory)
        {
            this.ti = ti;
            this.DbFactory = DbFactory;
        }

        public async Task<bool> CheckBusiness(int id)
        {
            DateTime now = ti.GetCurrentTime();
            try
            {
                Business b = new Business();
                string mail = "";
                string IsMail = "";
                List<string>? mails = new List<string>();
                using (var dbContext = DbFactory.CreateDbContext())
                {
                    b = dbContext.business.Find(id);
                    mail = dbContext.Users.First().Email;
                    mails = dbContext.email.Select(a => a.Email).ToList();
                    IsMail = dbContext.properties.Find(1).Value;
                }



                int x = (now - b.StartTracking).Days;

                if (x > 365)
                {
                    var weekly = JobManager.GetSchedule("Weekly" + b.Id);
                    if (weekly != null)
                    {
                        weekly.Disable();
                        using (var dbContext = DbFactory.CreateDbContext())
                        {
                            Business bb = dbContext.business.Find(b.Id);
                            bb.Status = "Completed";
                            dbContext.SaveChanges();
                        }
                        string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/BusinessMail.html");
                        body = body.Replace("[BusinessName]", b.BusinessName).Replace("[body]", "Task completed successfully! Reviews tracking for your business has fully passed it's tracking schedule.<br>").Replace("[RemainingTime]", "").Replace("[LocationUrl]", b.LocationLink).Replace("[Rating]", b.Rating.ToString());

                        MailMessage m = new MailMessage();
                        m.To.Add(mail);

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
                    return true;
                }



                DateTime OneWeekAgo = now.AddDays(-7).AddHours(-12);
                long TimeStamp = new DateTimeOffset(OneWeekAgo).ToUnixTimeSeconds();
                
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.app.outscraper.com/maps/reviews-v3?query=" + b.LocationLink + "&reviewsLimit=30&apiKey=XXXXXXXXXXXXXXXXXXXX&cutoff="+ TimeStamp);
                client.DefaultRequestHeaders.Add("X-API-KEY", "XXXXXXXXXXXXXXXXXXXXXXXXXX");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, "");
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                var Response1 = JsonConvert.DeserializeObject<ResponseContentModel>(responseContent);

                Thread.Sleep(10000);


            Resp2:
                HttpClient client2 = new HttpClient();
                client2.BaseAddress = new Uri(Response1.results_location);
                HttpRequestMessage httpRequest2 = new HttpRequestMessage(HttpMethod.Get, "");

                var response2 = await client2.SendAsync(httpRequest2);
                var responseContent2 = await response2.Content.ReadAsStringAsync();
                dynamic Response2 = JsonConvert.DeserializeObject<dynamic>(responseContent2);

                if (Response2.status == "Pending" || Response2.status == "pending")
                {
                    Thread.Sleep(5000);
                    goto Resp2;
                }

                string str = JsonConvert.SerializeObject(Response2.data);
                dynamic Data = JsonConvert.DeserializeObject<dynamic>(str);
                string ReviewDatastr = JsonConvert.SerializeObject(Data[0].reviews_data);
                dynamic[] Reviews = JsonConvert.DeserializeObject<dynamic[]>(ReviewDatastr);

                using (var dbContext = DbFactory.CreateDbContext())
                {
                    Business bb = dbContext.business.Find(b.Id);
                    bb.Rating = Data[0].rating;
                    bb.NumberOfReviews = Data[0].reviews;
                    bb.Week4 = bb.Week3;
                    bb.Week3 = bb.Week2;
                    bb.Week2 = bb.Week1;
                    bb.Week1 = Reviews.Length;
                    dbContext.SaveChanges();
                }



                if (Reviews.Length == 0)
                {
                    if (IsMail == "true")
                    {
                        string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/BusinessMail.html");
                        body = body.Replace("[BusinessName]", b.BusinessName).Replace("[body]", "Your business didn't have any reviews for the last week.<br>").Replace("[RemainingTime]", (365 - ((now - b.StartTracking)).Days).ToString()).Replace("[LocationUrl]", b.LocationLink).Replace("[Rating]", b.Rating.ToString());

                        MailMessage m = new MailMessage();
                        m.To.Add(mail);

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
                    }


                    return true;
                }
                else
                {

                    if (IsMail == "true")
                    {
                        string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/BusinessMail.html");
                        body = body.Replace("[BusinessName]", b.BusinessName).Replace("[body]", "Your business had "+ Reviews.Length + " new reviews this week.<br>").Replace("[RemainingTime]", (365 - ((now - b.StartTracking)).Days).ToString()).Replace("[LocationUrl]", b.LocationLink).Replace("[Rating]", b.Rating.ToString());


                        MailMessage m = new MailMessage();
                        m.To.Add(mail);
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
                    }

                    

                    return true;
                }

            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var inner = ex.InnerException.Message;
                var source = ex.Source;
                using (var dbContext = DbFactory.CreateDbContext())
                {
                    Properties p = new Properties();
                    p.Property = now.AddHours(8).ToString("dd MM yy - hh:mm tt");
                    p.Value = message + " -- " + inner;
                    dbContext.properties.Add(p);
                    dbContext.SaveChanges();
                }
                return false;
            }
        }

        public async Task<bool> CheckReview(int id)
        {
            DateTime now = ti.GetCurrentTime();
            try
            {
                Review r = new Review();
                string mail = "";
                string IsMail = "";
                List<string>? mails = new List<string>();
                using (var dbContext = DbFactory.CreateDbContext())
                {
                    r = dbContext.review.Find(id);
                    mail = dbContext.Users.First().Email;
                    mails = dbContext.email.Select(a => a.Email).ToList();
                    IsMail = dbContext.properties.Find(1).Value;
                }



                int x = (now - r.StartTracking).Days;

                if (x > 30)
                {
                   var one = JobManager.GetSchedule("OneDay" + r.Id);
                    if (one != null)
                    {
                        one.Disable();
                        using (var dbContext = DbFactory.CreateDbContext())
                        {
                            Review rr = dbContext.review.Find(r.Id);
                            rr.Status = "Completed";
                            dbContext.SaveChanges();
                        }
                        string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/Mail.html");
                        body = body.Replace("[ReviewText]", r.ReviewText).Replace("[body]", "Task completed successfully! your review for "+r.PlaceName+" has fully passed it's tracking schedule.<br>" ).Replace("[ReviewTime]", r.ReviewDateTime).Replace("[RemainingTime]", (7 - ((now - r.StartTracking)).Days).ToString()).Replace("[LocationUrl]", r.LocationUrl).Replace("[Rating]", r.Rating);

                        MailMessage m = new MailMessage();
                        m.To.Add(mail);

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
                else
                {
                    if (x > 60)
                    {
                        var sixh = JobManager.GetSchedule("SixHours" + r.Id);
                        if (sixh != null)
                        {
                            JobManager.GetSchedule("SixHours" + r.Id).Disable();
                        }
                        
                    }
                }
                
           



                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://api.app.outscraper.com/maps/reviews-v3?query=" + r.LocationUrl + "&reviewsLimit=1&apiKey=XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX&reviewsQuery=" + r.ReviewText);
                client.DefaultRequestHeaders.Add("X-API-KEY", "XXXXXXXXXXXXXXXXXXXXXXXXXX");
                HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, "");
                var response = await client.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();
                var Response1 = JsonConvert.DeserializeObject<ResponseContentModel>(responseContent);

                Thread.Sleep(10000);


            Resp2:
                HttpClient client2 = new HttpClient();
                client2.BaseAddress = new Uri(Response1.results_location);
                HttpRequestMessage httpRequest2 = new HttpRequestMessage(HttpMethod.Get, "");

                var response2 = await client2.SendAsync(httpRequest2);
                var responseContent2 = await response2.Content.ReadAsStringAsync();
                dynamic Response2 = JsonConvert.DeserializeObject<dynamic>(responseContent2);

                if (Response2.status == "Pending" || Response2.status == "pending")
                {
                    Thread.Sleep(5000);
                    goto Resp2;
                }

                string str = JsonConvert.SerializeObject(Response2.data);
                dynamic Data = JsonConvert.DeserializeObject<dynamic>(str);
                string ReviewDatastr = JsonConvert.SerializeObject(Data[0].reviews_data);
                dynamic[] Reviews = JsonConvert.DeserializeObject<dynamic[]>(ReviewDatastr);

                var length = Reviews.Length;
           
      
                if (length > 0)
                {
                    if (IsMail == "true")
                    {
                        string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/Mail.html");
                        body = body.Replace("[ReviewText]", r.ReviewText).Replace("[body]","Good news! your review for " + r.PlaceName + " still exists, we'll keep tracking it as per schedule<br>").Replace("[ReviewTime]", r.ReviewDateTime).Replace("[RemainingTime]", (7 - ((now - r.StartTracking)).Days).ToString()).Replace("[LocationUrl]", r.LocationUrl).Replace("[Rating]", r.Rating);

                        MailMessage m = new MailMessage();
                        m.To.Add(mail);

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
                    }

                    
                    return true;
                }
                else
                {

                    if (IsMail == "true")
                    {
                        string body = System.IO.File.ReadAllText(Directory.GetCurrentDirectory() + "/Templates/Mail.html");
                        body = body.Replace("[ReviewText]", r.ReviewText).Replace("[body]", "Attention required! your review for "+r.PlaceName+" Is no longer there as per our last check on<br>").Replace("[ReviewTime]", r.ReviewDateTime).Replace("[RemainingTime]", (7 - ((now - r.StartTracking)).Days).ToString()).Replace("[LocationUrl]", r.LocationUrl).Replace("[Rating]", r.Rating);


                        MailMessage m = new MailMessage();
                        m.To.Add(mail);

                        foreach (var item in mails)
                        {
                            m.To.Add(item);
                        }

                        m.Subject = r.PlaceName + " Review Removed";
                        m.From = new MailAddress("businesstracker@businesstracker.site");
                        m.Sender = new MailAddress("businesstracker@businesstracker.site");
                        m.IsBodyHtml = true;
                        m.Body = body;
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient("smtp.hostinger.com", 587);
                        smtp.EnableSsl = false;
                        smtp.Credentials = new NetworkCredential("businesstracker@businesstracker.site", "a6r^YQ@80:5R");
                        smtp.Send(m);
                    }

                    JobManager.GetSchedule("SixHours" + r.Id).Disable();
                    JobManager.GetSchedule("OneDay" + r.Id).Disable();
                    using (var dbContext = DbFactory.CreateDbContext())
                    {
                        Review rr = dbContext.review.Find(r.Id);
                        rr.Status = "Removed";
                        dbContext.SaveChanges();
                    }

                    return false;
                }


            }
            catch (Exception ex)
            {
                var message = ex.Message;
                var inner = ex.InnerException.Message;
                using (var dbContext = DbFactory.CreateDbContext())
                {
                    Properties p = new Properties();
                    p.Property = now.AddHours(8).ToString("dd MM yy - hh:mm tt");
                    p.Value = message + " -- " + inner;
                    dbContext.properties.Add(p);
                    dbContext.SaveChanges();
                }
                return false;
            }



        }

    }
}

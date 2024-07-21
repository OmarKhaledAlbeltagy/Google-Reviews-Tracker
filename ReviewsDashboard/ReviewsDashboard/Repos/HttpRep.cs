using Azure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Newtonsoft.Json;
using ReviewsDashboard.Context;
using ReviewsDashboard.Models;
using System.Net;

namespace ReviewsDashboard.Repos
{
    public class HttpRep : IHttpRep
    {
        private readonly DbContainer db;
        private readonly ITimeRep ti;
        public HttpRep(DbContainer db, ITimeRep ti)
        {
            this.db = db;
            this.ti = ti;
        }

        public async Task<string> GetBusiness(GetBusinessModel obj)
        {
            DateTime now = ti.GetCurrentTime();

            long weekago = new DateTimeOffset(now.AddDays(-7)).ToUnixTimeSeconds();

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.app.outscraper.com/maps/reviews-v3?query=" + obj.Link + "&reviewsLimit=1&apiKey=XXXXXXXXXXXXXXXXXXXXXXXXXX");
            client.DefaultRequestHeaders.Add("X-API-KEY", "XXXXXXXXXXXXXXXXXXXXXXXXXX");

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(httpRequest);

            var responseContent = await response.Content.ReadAsStringAsync();
            var Response1 = JsonConvert.DeserializeObject<ResponseContentModel>(responseContent);

            dynamic Response2;
            do
            {
                HttpClient client2 = new HttpClient();
                client2.BaseAddress = new Uri(Response1.results_location);
                HttpRequestMessage httpRequest2 = new HttpRequestMessage(HttpMethod.Get, "");
                var response2 = await client2.SendAsync(httpRequest2);
                var responseContent2 = await response2.Content.ReadAsStringAsync();
                Response2 = JsonConvert.DeserializeObject<dynamic>(responseContent2);
            } while (Response2.status == "Pending");



            return responseContent;
        }

        public string GetRedirectingUrl(string FirstUrl)
        {
            if (string.IsNullOrWhiteSpace(FirstUrl))
                return FirstUrl;

            int maxRedirCount = 8;  // prevent infinite loops
            string SecondUrl = FirstUrl;
            do
            {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(FirstUrl);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return SecondUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            SecondUrl = resp.Headers["Location"];
                            if (SecondUrl == null)
                                return FirstUrl;

                            if (SecondUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(FirstUrl), SecondUrl);
                                SecondUrl = u.ToString();
                            }
                            break;
                        default:
                            return SecondUrl;
                    }
                    FirstUrl = SecondUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return SecondUrl;
                }
                catch (Exception ex)
                {
                    return "";
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return SecondUrl;
        }

        public async Task<string> GetReviews(GetReviewModel obj)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://api.app.outscraper.com/maps/reviews-v3?query=" + obj.Link + "&reviewsLimit=10&apiKey=XXXXXXXXXXXXXXXXXXXXXXXXXX&reviewsQuery=" + obj.Review);
            client.DefaultRequestHeaders.Add("X-API-KEY", "XXXXXXXXXXXXXXXXXXXXXXXXXX");

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Get, "");

            var response = await client.SendAsync(httpRequest);
          
            var responseContent = await response.Content.ReadAsStringAsync();
            var Response1 = JsonConvert.DeserializeObject<ResponseContentModel>(responseContent);


            dynamic Response2;

            do
            {
                HttpClient client2 = new HttpClient();
                client2.BaseAddress = new Uri(Response1.results_location);
                HttpRequestMessage httpRequest2 = new HttpRequestMessage(HttpMethod.Get, "");
                var response2 = await client2.SendAsync(httpRequest2);
                var responseContent2 = await response2.Content.ReadAsStringAsync();
                Response2 = JsonConvert.DeserializeObject<dynamic>(responseContent2);
            } while (Response2.status == "Pending");

      



            return responseContent;
        }
    }
}

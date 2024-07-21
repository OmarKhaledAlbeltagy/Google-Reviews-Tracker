using ReviewsDashboard.Models;

namespace ReviewsDashboard.Repos
{
    public interface IHttpRep
    {
        string GetRedirectingUrl(string FirstUrl);

        Task<string> GetReviews(GetReviewModel obj);

        Task<string> GetBusiness(GetBusinessModel obj);
    }
}

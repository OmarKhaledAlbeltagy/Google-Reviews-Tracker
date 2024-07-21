using ReviewsDashboard.Entities;
using ReviewsDashboard.Models;

namespace ReviewsDashboard.Repos
{
    public interface IReviewTrackingRep
    {
        Task<bool> AddNewTracker(AddReviewTrack obj);

        List<InProgressModel> GetInProgress();

        List<InProgressModel> GetInHistory();

        bool StopTracking(int id);

        Task<bool> Retrack(int id);

        Task<bool> StartTrackAll();

        bool DeleteReview(int id);
    }
}

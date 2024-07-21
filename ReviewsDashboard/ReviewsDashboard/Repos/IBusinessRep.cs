using ReviewsDashboard.Models;

namespace ReviewsDashboard.Repos
{
    public interface IBusinessRep
    {
        Task<bool> AddNewBusiness(AddBusinessTrack obj);

        List<BusinessModel> GetInProgress();

        List<BusinessModel> GetInHistory();

        List<BusinessModel> GetPaused();

        bool PauseTracking(int id);

        Task<bool> ResumeTracking(int id);

        Task<bool> StartTrackAll();

        bool DeleteBusiness(int id);
    }
}

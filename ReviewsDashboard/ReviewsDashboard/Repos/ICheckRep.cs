namespace ReviewsDashboard.Repos
{
    public interface ICheckRep
    {
        Task<bool> CheckReview(int id);

        Task<bool> CheckBusiness(int id);
    }
}

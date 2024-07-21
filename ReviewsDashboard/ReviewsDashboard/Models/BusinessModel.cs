namespace ReviewsDashboard.Models
{
    public class BusinessModel
    {
        public int Id { get; set; }

        public string BusinessName { get; set; }

        public string ImgUrl { get; set; }

        public string LocationLink { get; set; }

        public int NumberOfReviews { get; set; }

        public int LastWeek { get; set; }

        public int LastMonth { get; set; }

        public double Rating { get; set; }

        public string Status { get; set; }

        public int DaysRemaining { get; set; }
    }
}

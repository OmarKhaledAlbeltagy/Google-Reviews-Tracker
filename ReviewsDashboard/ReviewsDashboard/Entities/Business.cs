namespace ReviewsDashboard.Entities
{
    public class Business
    {
        public int Id { get; set; }

        public string BusinessName { get; set; }

        public string ImgUrl { get; set; }

        public string LocationLink { get; set; }

        public int NumberOfReviews { get; set; }

        public int Week1 { get; set; } = 0;

        public int Week2 { get; set; } = 0;

        public int Week3 { get; set; } = 0;

        public int Week4 { get; set; } = 0;

        public double Rating { get; set; }

        public string Status { get; set; }

        public DateTime AddingDateTime { get; set; }

        public DateTime StartTracking { get; set; }

    }
}

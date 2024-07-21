namespace ReviewsDashboard.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public string PlaceName { get; set; }
        public string ReviewAuthor { get; set; }
        public string ReviewDateTime { get; set; }
        public string ReviewGoogleId { get; set; }
        public string ImageUrl { get; set; }
        public string LocationUrl { get; set; }
        public string Rating { get; set; }
        public string Status { get; set; }
        public DateTime AddingDateTime { get; set; }
        public DateTime StartTracking { get; set; }
    }
}

namespace ReviewsDashboard.Entities
{
    public class Emails
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public bool Sending { get; set; } = true;
    }
}

namespace CareConnect.Models
{
    public class ClaimImage
    {
        public int Id { get; set; }
        public int ClaimId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}

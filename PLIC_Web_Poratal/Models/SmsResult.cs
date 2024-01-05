namespace CareConnect.Models
{
    public class SmsResult
    {
        public string MobileNumber { get; set; }
        public string Status { get; set; } = string.Empty;
        public string statusCode { get; set; }
        public string messages { get; set; }
    }
}

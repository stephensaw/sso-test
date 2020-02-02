namespace SSO.Models
{
    public class ApplicationModel
    {
        public long Id { get; set; }
        public string RedirectUrl { get; set; }
        public string Token { get; set; }
    }
}

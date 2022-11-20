namespace WcSync.Wc
{
    public class WcConfiguration
    {
        public string Host { get; set; }
        public string Client { get; set; }
        public string Secret { get; set; }
        public int? TotalPages { get; set; }
        public int? RequestDelay { get; set; }
        public int? FailedRequestDelay { get; set; }
    }
}
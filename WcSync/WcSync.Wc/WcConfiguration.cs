namespace WcSync.Wc
{
    public class WcConfiguration
    {
        required public string Host { get; init; }
        required public string Client { get; init; }
        required public string Secret { get; init; }
        public int? TotalPages { get; init; }
        public int? RequestDelay { get; init; }
        public int? FailedRequestDelay { get; init; }
    }
}
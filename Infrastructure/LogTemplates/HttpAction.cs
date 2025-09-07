namespace Infrastructure
{
    public class HttpAction
    {
        public required string FromIp { get; set; }

        public required string User { get; set; }

        public required double Duration { get; set; }

        public required int StatusCode { get; set; }

        public required string Method { get; set; }

        public string? RequestData { get; set; }

        public string? ResponseData { get; set; }
    }
}
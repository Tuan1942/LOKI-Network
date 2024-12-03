namespace LOKI_Network.Helpers
{
    public class UrlHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetServerUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
                throw new InvalidOperationException("Request is not available.");

            var scheme = request.Scheme; // e.g., http or https
            var host = request.Host;     // e.g., localhost:5000 or 192.168.1.112:2231

            return $"{scheme}://{host}/";
        }
    }
}

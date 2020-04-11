using System.Net;

namespace SpotifyFunTime.Contracts
{
    public class ApiResponse<T>
    {
        public ApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
        }
        
        public HttpStatusCode StatusCode { get; set; }
        public T Content { get; set; }
        public string ReasonPhrase { get; set; }

        public bool IsSuccessStatusCode => StatusCode == HttpStatusCode.OK;
    }
}
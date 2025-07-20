using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mocks
{
    public class GpsCoordinateSender
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;

        public GpsCoordinateSender(string apiBaseUrl)
        {
            _httpClient = new HttpClient();
            _endpoint = $"{apiBaseUrl.TrimEnd('/')}/api/geolocation/store-coordinate";
        }

        public async Task<bool> SendCoordinateAsync(object coordinate)
        {
            var content = new StringContent(JsonSerializer.Serialize(coordinate), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_endpoint, content);
            return response.IsSuccessStatusCode;
        }
    }
}
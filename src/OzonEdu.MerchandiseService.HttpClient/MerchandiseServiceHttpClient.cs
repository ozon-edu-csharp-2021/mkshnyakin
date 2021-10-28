using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using OzonEdu.MerchandiseService.HttpModels;
using SystemHttpClient = System.Net.Http.HttpClient;

namespace OzonEdu.MerchandiseService.HttpClient
{
    public class MerchandiseServiceHttpClient : IMerchandiseServiceHttpClient
    {
        private readonly SystemHttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public MerchandiseServiceHttpClient(SystemHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EmployeeMerchGetResponse> V1GetHistoryForEmployee(int employeeId, CancellationToken token)
        {
            var path = $"/api/v1/employee/{employeeId}/merch";
            using var response = await _httpClient.GetAsync(path, token);
            var json = await response.Content.ReadAsStringAsync(token);
            var result = JsonSerializer.Deserialize<EmployeeMerchGetResponse>(json, _jsonSerializerOptions);
            return result;
        }

        public async Task<EmployeeMerchPostResponse> V1RequestMerchForEmployee(int employeeId, CancellationToken token)
        {
            var path = $"/api/v1/employee/{employeeId}/merch";
            using var response = await _httpClient.PostAsync(path, null!, token);
            var json = await response.Content.ReadAsStringAsync(token);
            var result = JsonSerializer.Deserialize<EmployeeMerchPostResponse>(json, _jsonSerializerOptions);
            return result;
        }
    }
}
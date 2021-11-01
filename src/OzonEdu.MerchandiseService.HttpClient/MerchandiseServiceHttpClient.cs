using System;
using System.Linq;
using System.Net;
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
            EmployeeMerchGetResponse result = null;
            try
            {
                using var response = await _httpClient.GetAsync(path, token);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                    {
                        var json = await response.Content.ReadAsStringAsync(token);
                        result = JsonSerializer.Deserialize<EmployeeMerchGetResponse>(json, _jsonSerializerOptions);
                        break;
                    }
                    case HttpStatusCode.NotFound:
                        try
                        {
                            var json = await response.Content.ReadAsStringAsync(token);
                            var error = JsonSerializer.Deserialize<RestErrorResponse>(json, _jsonSerializerOptions);
                            if (error is not null)
                            {
                                result = new EmployeeMerchGetResponse
                                {
                                    Items = Enumerable.Empty<EmployeeMerchGetResponseItem>()
                                };
                            }
                        }
                        catch (Exception e)
                        {
                        }
                        break;
                }
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public async Task<EmployeeMerchPostResponse> V1RequestMerchForEmployee(int employeeId, CancellationToken token)
        {
            var path = $"/api/v1/employee/{employeeId}/merch";
            EmployeeMerchPostResponse result = null;
            try
            {
                using var response = await _httpClient.PostAsync(path, null!, token);
                var json = await response.Content.ReadAsStringAsync(token);
                result = JsonSerializer.Deserialize<EmployeeMerchPostResponse>(json, _jsonSerializerOptions);
            }
            catch (Exception e)
            {
            }
            return result;
        }
    }
}
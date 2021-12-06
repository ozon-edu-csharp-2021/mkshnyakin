using System;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using OpenTracing;
using OzonEdu.MerchandiseService.Infrastructure.Configuration;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Stubs;
using SystemHttpClient = System.Net.Http.HttpClient;
using CSharpCourseEmployeeViewModel = CSharpCourse.EmployeesService.PresentationModels.Employees.EmployeeViewModel;
using CSharpCourseEmployeesViewModel = CSharpCourse.EmployeesService.PresentationModels.Employees.EmployeesViewModel;

namespace OzonEdu.MerchandiseService.Infrastructure.Clients.Implementation
{
    public class OzonEduEmployeeServiceHttpClient : IOzonEduEmployeeServiceClient
    {
        private readonly SystemHttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly OzonEduEmployeeServiceHttpOptions _options;
        private readonly ITracer _tracer;

        public OzonEduEmployeeServiceHttpClient(IOptions<OzonEduEmployeeServiceHttpOptions> options, ITracer tracer)
        {
            _tracer = tracer;
            _options = options.Value;
            _httpClient = new SystemHttpClient
            {
                BaseAddress = new Uri(_options.BaseAddress)
            };
        }

        public int TryMax { get; set; } = 3;
        public int TryDelayMs { get; set; } = 333;

        public async Task<OzonEduEmployeeServiceClient.EmployeeViewModel> GetByIdAsync(
            long employeeId,
            CancellationToken cancellationToken = default)
        {
            using var span = _tracer
                .BuildSpan($"{nameof(OzonEduEmployeeServiceHttpClient)}.{nameof(GetByIdAsync)}")
                .StartActive();

            var path = $"/api/employees/{employeeId}";
            OzonEduEmployeeServiceClient.EmployeeViewModel result = default;
            try
            {
                using var response = await _httpClient.GetAsync(path, cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    var employeeViewModel =
                        JsonSerializer.Deserialize<CSharpCourseEmployeeViewModel>(json, _jsonOptions);
                    if (employeeViewModel != null) result = MapEmployeeViewModel(employeeViewModel);
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        public async Task<OzonEduEmployeeServiceClient.EmployeeViewModel> GetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default
        )
        {
            using var span = _tracer
                .BuildSpan($"{nameof(OzonEduEmployeeServiceHttpClient)}.{nameof(GetByEmailAsync)}")
                .StartActive();

            // Грязный хак!   
            // Так как в CSharpCourse.EmployeesService.ApplicationServices.Handlers.Employees.CreateEmployeeCommandHandler.Handle
            // событие сперва отправляется в топик, а только потом комитится транзакция в БД,
            // то с первого раза можно не найти созданного сотрудника

            var tryCount = 1;
            while (tryCount++ <= TryMax)
            {
                span.Span.SetTag(nameof(tryCount), tryCount);
                var result = await TryGetByEmailAsync(email, cancellationToken);
                if (result != null)
                {
                    span.Span.SetTag("success", true);
                    return result;
                }

                await Task.Delay(TryDelayMs, cancellationToken);
            }

            span.Span.SetTag("success", false);
            return null;
        }

        private async Task<OzonEduEmployeeServiceClient.EmployeeViewModel> TryGetByEmailAsync(
            string email,
            CancellationToken cancellationToken = default)
        {
            var path = "/api/employees";
            OzonEduEmployeeServiceClient.EmployeeViewModel result = default;
            try
            {
                using var response = await _httpClient.GetAsync(path, cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var json = await response.Content.ReadAsStringAsync(cancellationToken);
                    var httpResult = JsonSerializer.Deserialize<CSharpCourseEmployeesViewModel>(json, _jsonOptions);
                    var items = httpResult?.Items ?? Enumerable.Empty<CSharpCourseEmployeeViewModel>();
                    var employeeViewModel = items.FirstOrDefault(x => x.Email == email);
                    if (employeeViewModel != default) result = MapEmployeeViewModel(employeeViewModel);
                }
            }
            catch (Exception)
            {
            }

            return result;
        }

        private OzonEduEmployeeServiceClient.EmployeeViewModel MapEmployeeViewModel(
            CSharpCourseEmployeeViewModel viewModel)
        {
            return new OzonEduEmployeeServiceClient.EmployeeViewModel
            {
                Id = viewModel.Id,
                Email = viewModel.Email,
                BirthDay = viewModel.BirthDay,
                FirstName = viewModel.FirstName,
                MiddleName = viewModel.MiddleName,
                LastName = viewModel.LastName,
                HiringDate = viewModel.HiringDate
            };
        }
    }
}
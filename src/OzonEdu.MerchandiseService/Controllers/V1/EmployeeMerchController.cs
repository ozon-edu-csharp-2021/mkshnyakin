using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OzonEdu.MerchandiseService.HttpModels;
using OzonEdu.MerchandiseService.Services;

namespace OzonEdu.MerchandiseService.Controllers.V1
{
    [ApiController]
    [Route("api/v1/employee/{employeeId:int}/merch")]
    [Produces("application/json")]
    public class EmployeeMerchController : ControllerBase
    {
        private readonly IMerchandiseService _merchandiseService;

        public EmployeeMerchController(IMerchandiseService merchandiseMerchandiseService)
        {
            _merchandiseService = merchandiseMerchandiseService;
        }

        [HttpGet]
        public async Task<ActionResult<EmployeeMerchGetResponse>> GetHistoryForEmployee(
            int employeeId,
            CancellationToken token)
        {
            var items = await _merchandiseService.GetHistoryForEmployee(employeeId, token);
            if (items is null)
            {
                return NotFound();
            }

            var response = new EmployeeMerchGetResponse
            {
                Items = items
                    .Select(item => new EmployeeMerchGetResponseItem
                    {
                        Item = new EmployeeMerchItem
                        {
                            Name = item.Item.Name,
                            SkuId = item.Item.SkuId
                        },
                        Date = item.Date
                    })
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeMerchPostResponse>> RequestMerchForEmployee(
            int employeeId,
            CancellationToken token)
        {
            var items = await _merchandiseService.RequestMerchForEmployee(employeeId, token);
            if (items is null)
            {
                return Conflict();
            }

            var response = new EmployeeMerchPostResponse
            {
                Items = items.Select(item => new EmployeeMerchItem
                {
                    Name = item.Name,
                    SkuId = item.SkuId
                })
            };

            return Ok(response);
        }
    }
}
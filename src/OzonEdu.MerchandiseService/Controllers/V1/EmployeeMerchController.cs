using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Запрос ранее выданного мерча сотруднику
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeMerchGetResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                    .Select(x => new EmployeeMerchGetResponseItem
                    {
                        Item = new EmployeeMerchItem
                        {
                            Name = x.Item.Name,
                            SkuId = x.Item.SkuId
                        },
                        Date = x.Date
                    })
            };

            return Ok(response);
        }

        /// <summary>
        /// Запрос на выдачу мерча для сотрудника
        /// </summary>
        /// <param name="employeeId">Идентификатор сотрудника</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(EmployeeMerchPostResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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
                Items = items.Select(x => new EmployeeMerchItem
                {
                    Name = x.Name,
                    SkuId = x.SkuId
                })
            };

            return Ok(response);
        }
    }
}
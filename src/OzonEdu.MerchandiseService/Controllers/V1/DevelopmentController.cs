using System;
using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using CSharpCourse.Core.Lib.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OzonEdu.MerchandiseService.Infrastructure.Commands;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Commands.SupplyEvent;

namespace OzonEdu.MerchandiseService.Controllers.V1
{
    [ApiController]
    [Route("dev/")]
    [Produces("application/json")]
    public class DevelopmentController : ControllerBase
    {
        public enum EmployeeIdEnum
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Five = 5,
            Six = 6,
            Seven = 7,
            Eight = 8,
            Nine = 9,
            Ten = 10
        }

        private readonly ILogger<DevelopmentController> _logger;

        private readonly IMediator _mediator;

        public DevelopmentController(IMediator mediator, ILogger<DevelopmentController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        ///     Симуляция автоматического (системного) запроса на выдачу мерча
        /// </summary>
        /// <param name="employeeIdEnum">Заглушка на 10 employeeId</param>
        /// <param name="employeeEventType">
        ///     Hiring=100, ProbationPeriodEnding=200, ConferenceAttendance=300,
        ///     MerchDelivery=400, Dismissal=1000
        /// </param>
        /// <param name="merchType">
        ///     WelcomePack=10, ConferenceListenerPack=20, ConferenceSpeakerPack=30,
        ///     ProbationPeriodEndingPack=40, VeteranPack = 50
        /// </param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("send-system-merch-request/{employeeIdEnum}/{merchType}")]
        public async Task<ActionResult> SendSystemMerchRequest(
            EmployeeIdEnum employeeIdEnum,
            MerchType merchType,
            CancellationToken token)
        {
            try
            {
                var employeeId = (long) employeeIdEnum;

                var processMerchRequestCommand = new ProcessMerchRequestCommand
                {
                    EmployeeId = employeeId,
                    MerchType = merchType,
                    IsSystem = true
                };

                await _mediator.Send(processMerchRequestCommand, token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not process system merch request");
                return UnprocessableEntity();
            }

            return Ok();
        }

        /// <summary>
        ///     Симуляция события поставки
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ship-supply")]
        public async Task<ActionResult> ShipSupply(CancellationToken token)
        {
            try
            {
                var processSupplyEventCommand = new ProcessSupplyEventCommand
                {
                    Items = new SupplyShippedItem[]
                    {
                        new() {SkuId = 2, Quantity = 10}, // ConferenceListenerPack, ConferenceSpeakerPack
                        new() {SkuId = 4, Quantity = 10}, // ProbationPeriodEndingPack
                        new() {SkuId = 5, Quantity = 10}, // ProbationPeriodEndingPack
                        new() {SkuId = 6, Quantity = 10}, // VeteranPack
                        new() {SkuId = 7, Quantity = 10}, // VeteranPack
                        new() {SkuId = 8, Quantity = 10}
                    }
                };
                await _mediator.Send(processSupplyEventCommand, token);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Could not process supply event");
                return UnprocessableEntity();
            }

            return Ok();
        }
        
        /// <summary>
        ///     Тестирование реализации репозиториев
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("test-repositories")]
        public async Task<ActionResult> TestRepositories(CancellationToken token)
        {
            var testCommand = new TestCommand();
            await _mediator.Send(testCommand, token);

            return Ok();
        }
    }
}
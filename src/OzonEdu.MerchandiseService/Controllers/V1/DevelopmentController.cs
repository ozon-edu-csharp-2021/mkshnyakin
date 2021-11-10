using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using CSharpCourse.Core.Lib.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OzonEdu.MerchandiseService.Infrastructure.Commands.MerchRequestAggregate;
using OzonEdu.MerchandiseService.Infrastructure.Contracts;

namespace OzonEdu.MerchandiseService.Controllers.V1
{
    [ApiController]
    [Route("dev/")]
    [Produces("application/json")]
    public class DevelopmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IOzonEduEmployeeServiceClient _employeeServiceClient;

        public DevelopmentController(IMediator mediator, IOzonEduEmployeeServiceClient employeeServiceClient)
        {
            _mediator = mediator;
            _employeeServiceClient = employeeServiceClient;
        }

        /// <summary>
        /// Симуляция события поставки
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ship-supply")]
        public async Task<ActionResult> ShipSupply(CancellationToken token)
        {
            return Ok();
        }

        /// <summary>
        ///  Симуляция события сотрудника
        /// </summary>
        /// <param name="employeeIdEnum">Заглушка на 10 employeeId</param>
        /// <param name="employeeEventType">Hiring=100, ProbationPeriodEnding=200, ConferenceAttendance=300,
        /// MerchDelivery=400, Dismissal=1000</param>
        /// <param name="merchType">WelcomePack=10, ConferenceListenerPack=20, ConferenceSpeakerPack=30, ProbationPeriodEndingPack=40, VeteranPack = 50</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("send-employee-event/{employeeIdEnum}/{merchType}")]
        public async Task<ActionResult> SendEmployeeEvent(
            EmployeeIdEnum employeeIdEnum,
            MerchType merchType,
            CancellationToken token)
        {
            var employeeId = (long) employeeIdEnum;
            
            var createMerchRequestForEmployeeIdCommand = new CreateMerchRequestForEmployeeIdCommand
            {
                EmployeeId = employeeId,
                MerchType = merchType,
                IsSystem = true,
            };
            
            await _mediator.Send(createMerchRequestForEmployeeIdCommand, token);
            
            return Ok();
        }

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
    }
}
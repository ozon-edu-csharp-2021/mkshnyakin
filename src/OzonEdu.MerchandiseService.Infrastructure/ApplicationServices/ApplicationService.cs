using OzonEdu.MerchandiseService.Infrastructure.Contracts;
using OzonEdu.MerchandiseService.Infrastructure.Contracts.MessageBus;

namespace OzonEdu.MerchandiseService.Infrastructure.ApplicationServices
{
    public sealed class ApplicationService
    {
        /// <summary>
        /// Интерфейс репозитория БД
        /// </summary>
        private readonly IOzonEduEmployeeServiceClient _employeeClient;

        /// <summary>
        /// Интерфейс шины сообщений
        /// </summary>
        private readonly IMessageBus _messageBus;

        public ApplicationService(IOzonEduEmployeeServiceClient employeeClient, IMessageBus messageBus)
        {
            _employeeClient = employeeClient;
            _messageBus = messageBus;
        }

        /// <summary>
        /// Создать запрос на выдачу мерча новому сотруднику
        /// </summary>
        public GiveMerchResult GiveMerchForNewEmployee(string newEmployeeEmail)
        {
            var employee = _employeeClient.FindEmployeeByEmail(newEmployeeEmail);

            if (employee == null)
            {
                return GiveMerchResult.Fail("Employee not found");
            }

            var managers = _employeeClient.GetVacantManagers().ToList();

            try
            {
                var request = DomainService.CreateMerchandizeRequest(employee, managers);
                //var responsibleManager = managers.First(m => m.Id == request.ResponsibleManagerId);

                //_messageBus.Notify(new EmailMessage(responsibleManager.Email, "Надо выдать мерч"));
                var employeeEmailMessage = new EmailMessage
                {
                    ToEmail = employee.Email,
                    ToName =  employee.Name,
                    Subject = "Вам будет выдан мерч",
                    Body = string.Empty
                };
                _messageBus.Notify(employeeEmailMessage);

                return GiveMerchResult.Success(request.Status, "Task has been assigned to a manager", request.Id);
            }
            catch (Exception e)
            {
                return GiveMerchResult.Fail(e.Message);
            }
        }
    }
}
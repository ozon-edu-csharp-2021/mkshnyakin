using System.Collections.Generic;
using CSharpCourse.Core.Lib.Models;
using MediatR;

namespace OzonEdu.MerchandiseService.Infrastructure.Commands.SupplyEvent
{
    public class ProcessSupplyEventCommand : IRequest
    {
        public ICollection<SupplyShippedItem> Items { get; set; }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IClaimTemplateService
    {
        Task CreateAllClaims();
    }
}


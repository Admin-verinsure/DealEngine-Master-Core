using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IClaimTemplateService
    {
        Task CreateClaimsForOrganisation(Organisation primaryOrganisation);
    }
}


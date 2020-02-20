using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IImportService
    { 
        Task ImportAOEService(User user);
        Task ImportAOEServiceIndividuals(User user);
        Task ImportAOEServicePrincipals(User user);
        Task ImportAOEServiceBusinessContract(User user);
        Task ImportAOEServiceClaims(User user);
        Task ImportActivities(User user);
    }
}

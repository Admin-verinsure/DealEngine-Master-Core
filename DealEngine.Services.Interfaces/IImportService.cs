using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IImportService
    { 
        Task ImportAOEService(User user);
        Task ImportAOEServiceIndividuals(User user);
        Task ImportAOEServicePrincipals(User user);
        Task ImportAOEServiceBusinessContract(User user);
        Task ImportAOEServiceClaims(User user);
        Task ImportActivities(User user);
        Task ImportCEASServiceIndividuals(User user);
        Task ImportCEASServiceClaims(User user);
    }
}

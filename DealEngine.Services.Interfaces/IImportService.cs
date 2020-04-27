using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IImportService
    { 
        Task ImportAOEServiceIndividuals(User user);
        Task ImportAOEServicePrincipals(User user);
        Task ImportAOEServiceContract(User user);
        Task ImportAOEServiceClaims(User user);
        Task ImportActivities(User user);
        Task ImportCEASServiceIndividuals(User user);
        Task ImportPMINZServiceIndividuals(User user);
        Task ImportCEASServiceClaims(User user);
        Task ImportCEASServiceContract(User user);
        Task ImportPMINZServiceContract(User user);
        Task ImportCEASServicePrincipals(User user);
        Task ImportPMINZServicePrincipals(User user);
        Task ImportPMINZServicePreRenewData(User user);
        Task ImportDANZServicePreRenewData(User user);
        Task ImportDANZServiceIndividuals(User user);
        Task ImportDANZServicePersonnel(User user);
    }
}

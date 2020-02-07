using System;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IAppSettingService
    {
        string CarJamEndpoint { get; }
        string CarJamApiKey { get; }
        string IntermediatePassword { get; }
        string domainQueryString { get; }
        string RequireRSA { get; }
        string GetMarineInsuranceSpecialistEmail { get; }
        string GetCompanyTitle { get; }
        string GetConnectionString { get; }
        string GetSuperUser { get; }
    }
}

 

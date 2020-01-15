using TechCertain.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace TechCertain.Services.Impl
{
    public class AppSettingService : IAppSettingService
    {
        private IConfiguration _configuration { get; set; }
        public AppSettingService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CarJamEndpoint
        {
            get
            {
                return _configuration.GetValue<string>("CarJamEndpoint");
            }
        }

        public string CarJamApiKey
        {
            get
            {
                return _configuration.GetValue<string>("CarJamApiKey");
            }
        }

        public string IntermediatePassword
        {
            get
            {
                return _configuration.GetValue<string>("IntermediatePassword");
            }
        }

        public string domainQueryString
        {
            get
            {
                return _configuration.GetValue<string>("domainQueryString");
            }
        }

        public bool RequireRSA
        {
            get
            {
                return _configuration.GetValue<bool>("RequireRSA");
            }
        }

        public string GetMarineInsuranceSpecialistEmail
        {
            get
            {
                return _configuration.GetValue<string>("MarineInsuranceSpecialistEmail");
            }
        }

        public string GetCompanyTitle
        {
            get
            {
                return _configuration.GetValue<string>("GetCompanyTitle");
            }
        }

        public string GetConnectionString
        {
            get
            {
                return _configuration.GetConnectionString("TechCertainConnection");
            }
        }
    }
}

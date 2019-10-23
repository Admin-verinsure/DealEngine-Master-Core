﻿using TechCertain.Services.Interfaces;
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


    }
}
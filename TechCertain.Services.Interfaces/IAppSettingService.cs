﻿using System;
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
        bool RequireRSA { get; }
    }
}

 

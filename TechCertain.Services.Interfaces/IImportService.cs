﻿using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IImportService
    { 
        Task ImportAOEService(User user);
        Task ImportActivities(User user);
    }
}
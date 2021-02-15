using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IAdvisoryService
    {
        Task CreateAdvisory(Advisory advisory);
        Task UpdateAdvisory(Advisory advisory);
    }
}

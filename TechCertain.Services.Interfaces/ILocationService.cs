using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ILocationService
    {
        Task<Location> GetLocationByStreet(string street);
        Task<Location> GetLocationById(Guid locationId);
        Task<List<string>> GetLocationStreetList();
    }
}

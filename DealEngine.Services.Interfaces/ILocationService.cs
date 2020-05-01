using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface ILocationService
    {
        Task<Location> GetLocationByStreet(string street);
        Task<Location> GetLocationById(Guid locationId);
        Task<List<string>> GetLocationStreetList();
        Task UpdateLocation(Location location);
    }
}

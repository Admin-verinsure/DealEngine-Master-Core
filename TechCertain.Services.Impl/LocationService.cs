using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TechCertain.Services.Impl
{
    public class LocationService : ILocationService
    {
        IMapperSession<Location> _locationRepository;

        public LocationService(IMapperSession<Location> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<Location> GetLocationById(Guid locationId)
        {
            return await _locationRepository.GetByIdAsync(locationId);
        }

        public async Task<Location> GetLocationByStreet(string street)
        {
            return await _locationRepository.FindAll().FirstOrDefaultAsync(l => l.Street == street);
        }

        public async Task<List<string>> GetLocationStreetList()
        {
            return await _locationRepository.FindAll().Select(l => l.Street).ToListAsync();
        }
    }
}


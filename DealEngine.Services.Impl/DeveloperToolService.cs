using DealEngine.Services.Interfaces;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Domain.Entities;
using System.Threading.Tasks;
using NHibernate.Linq;
using System;

namespace DealEngine.Services.Impl
{
    public class DeveloperToolService : IDeveloperToolService
    {
        IMapperSession<WaterLocation> _WaterLocationRepository;

        public DeveloperToolService(IMapperSession<WaterLocation> WaterLocationRepository)
        {
            _WaterLocationRepository = WaterLocationRepository;
        }

        public async Task CreateMarinas()
        {
            WaterLocation defaultMar1 = new WaterLocation(null);
            defaultMar1.MarinaName = "DefaultMar1";
            defaultMar1.IsPublic = true;
            _WaterLocationRepository.AddAsync(defaultMar1);
            WaterLocation DefaultMar2 = new WaterLocation(null);
            DefaultMar2.MarinaName = "DefaultMar2";
            DefaultMar2.IsPublic = true;
            _WaterLocationRepository.AddAsync(DefaultMar2);
            WaterLocation DefaultMar3 = new WaterLocation(null);
            DefaultMar3.MarinaName = "DefaultMar3";
            DefaultMar3.IsPublic = true;
            _WaterLocationRepository.AddAsync(DefaultMar3);
            WaterLocation DefaultMar4 = new WaterLocation(null);
            DefaultMar4.MarinaName = "DefaultMar4";
            DefaultMar4.IsPublic = true;
            _WaterLocationRepository.AddAsync(DefaultMar4);
            WaterLocation DefaultMar5 = new WaterLocation(null);
            DefaultMar5.MarinaName = "DefaultMar5";
            DefaultMar5.IsPublic = true;
            _WaterLocationRepository.AddAsync(DefaultMar5);
        }
    }
}


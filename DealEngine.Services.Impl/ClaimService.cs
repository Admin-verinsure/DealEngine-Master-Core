﻿using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class ClaimService : IClaimService
    {
        IMapperSession<Domain.Entities.Claim> _claimsRepository;

        public ClaimService(IMapperSession<Domain.Entities.Claim> claimsRepository)
        {
            _claimsRepository = claimsRepository;
        }

        public async Task AddClaim(Domain.Entities.Claim claim)
        {
            await _claimsRepository.AddAsync(claim);
        }

        public async Task<List<Domain.Entities.Claim>> GetClaimsAllClaimsList()
        {
            return await _claimsRepository.FindAll().ToListAsync();
        }

        public async Task<Domain.Entities.Claim> GetTemplateByName(string claimName)
        {
            return await _claimsRepository.FindAll().FirstOrDefaultAsync(c => c.Value == claimName);
        }
    }
}

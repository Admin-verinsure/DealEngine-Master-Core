using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class SharedDataRoleService : ISharedDataRoleService
    {
        IMapperSession<SharedDataRole> _sharedDataRoleRepository;
        IMapperSession<SharedDataRoleTemplate> _sharedDataRoleTemplateRepository;

        public SharedDataRoleService(IMapperSession<SharedDataRole> businessActivityRepository, IMapperSession<SharedDataRoleTemplate> businessActivityTemplateRepository)
        {
            _sharedDataRoleRepository = businessActivityRepository;
            _sharedDataRoleTemplateRepository = businessActivityTemplateRepository;
        }

        public async Task CreateSharedDataRole(SharedDataRole newSharedRole)
        {
            await _sharedDataRoleRepository.AddAsync(newSharedRole);
        }

        public async Task CreateSharedDataRoleTemplate(SharedDataRoleTemplate newSharedRole)
        {
            await _sharedDataRoleTemplateRepository.AddAsync(newSharedRole);
        }

        public async Task<List<SharedDataRoleTemplate>> GetRolesByOwner(Guid organisationId)
        {
            return await _sharedDataRoleTemplateRepository.FindAll().Where(r => r.Organisation.Id == organisationId || r.IsPublic == true).ToListAsync();
        }

        public async Task<SharedDataRoleTemplate> GetSharedRoleTemplateById(Guid Id)
        {
            return await _sharedDataRoleTemplateRepository.GetByIdAsync(Id);
        }

        public async Task<SharedDataRoleTemplate> GetSharedRoleTemplateByRoleName(string Name)
        {
            return await _sharedDataRoleTemplateRepository.FindAll().FirstOrDefaultAsync(s => s.Name == Name);
        }

        public async Task<List<SharedDataRoleTemplate>> GetSharedRoleTemplatesByProgramme(Programme Programme)
        {
            return await _sharedDataRoleTemplateRepository.FindAll().Where(s => s.Programme == Programme).ToListAsync();
        }

        public async Task UpdateSharedRole(SharedDataRole sharedRole)
        {
            await _sharedDataRoleRepository.UpdateAsync(sharedRole);
        }
    }
}

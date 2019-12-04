
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface ISharedDataRoleService
    {
        Task<List<SharedDataRoleTemplate>> GetRolesByOwner(Guid id);
        Task CreateSharedDataRoleTemplate(SharedDataRoleTemplate newSharedRole);
        Task<SharedDataRoleTemplate> GetSharedRoleTemplateById(Guid Id);
        Task<SharedDataRoleTemplate> GetSharedRoleTemplateByRoleName(string Name);
        Task<List<SharedDataRoleTemplate>> GetSharedRoleTemplatesByProgramme(Programme Programme);
        Task CreateSharedDataRole(SharedDataRole newSharedRole);
        Task UpdateSharedRole(SharedDataRole sharedRole);
    }

}

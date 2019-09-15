using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IRoleService
    {
        IQueryable GetRoles();
        void CreateRole(string title);
        Role GetRole(Guid roleId);
        void AttachClientProgrammeToRole(Programme programme, Role role);
        IEnumerable<Role> GetRolesByProgramme(Guid programmeId);
    }
}


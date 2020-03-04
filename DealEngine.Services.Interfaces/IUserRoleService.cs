using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using NHibernate.AspNetCore.Identity;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IUserRoleService
    {
        Task AddUserRole(User user, IdentityRole role, Organisation organisation);
        Task<List<UserRole>> GetRolesByOrganisation(Organisation primaryOrganisation);
    }
}


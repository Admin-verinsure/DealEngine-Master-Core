using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class UserRoleService : IUserRoleService
    {
        IMapperSession<UserRole> _userRoleRepository;

        public UserRoleService(IMapperSession<UserRole> userRoleRepository)
        {
            _userRoleRepository = userRoleRepository;
        }

        public async Task AddUserRole(User user, IdentityRole role, Organisation organisation)
        {
            UserRole userRole = new UserRole(user)
            {
                IdentityRoleName = role.Name,
                Organisation = organisation
            };

            await _userRoleRepository.AddAsync(userRole);
        }

        public async Task<List<UserRole>> GetRolesByOrganisation(Organisation primaryOrganisation)
        {
            return await _userRoleRepository.FindAll().Where(ur => ur.Organisation == primaryOrganisation).ToListAsync();
        }
    }
}

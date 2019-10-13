using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class RoleService : IRoleService
    {
        IMapperSession<Role> _roleRepository;

        public RoleService(IMapperSession<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public void AttachClientProgrammeToRole(Programme programme, Role role)
        {
            role.Programme = programme;
            Update(role);
        }

        private void Update(Role role)
        {
            _roleRepository.UpdateAsync(role);
        }

        public void CreateRole(string title)
        {
            Role role = new Role();
            role.Title = title;
            _roleRepository.AddAsync(role);
        }

        public Role GetRole(Guid roleId)
        {
            return _roleRepository.GetById(roleId).Result;
        }

        public IQueryable GetRoles()
        {
            return _roleRepository.FindAll();
        }

        public IEnumerable<Role> GetRolesByProgramme(Guid programmeId)
        {
            return _roleRepository.FindAll().Where(r => r.Programme.Id == programmeId);
        }
    }
}


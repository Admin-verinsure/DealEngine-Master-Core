using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class RoleService : IRoleService
    {
        IRepository<Role> _roleRepository;
        IUnitOfWorkFactory _uowFactory;

        public RoleService(IRepository<Role> roleRepository, IUnitOfWorkFactory uowFactory)
        {
            _roleRepository = roleRepository;
            _uowFactory = uowFactory;
        }

        public void AttachClientProgrammeToRole(Programme programme, Role role)
        {
            role.Programme = programme;
            Update(role);
        }

        private void Update(Role role)
        {
            using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork())
            {
                _roleRepository.Update(role);
                uow.Commit();
            }
        }

        public void CreateRole(string title)
        {
            Role role = new Role();
            role.Title = title;

            using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork())
            {
                _roleRepository.Add(role);
                uow.Commit();
            }
        }

        public Role GetRole(Guid roleId)
        {
            return _roleRepository.GetById(roleId);
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


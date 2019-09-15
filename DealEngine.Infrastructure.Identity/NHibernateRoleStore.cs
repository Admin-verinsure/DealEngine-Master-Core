using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.Identity
{
	public class NHibernateRoleStore : IDealEngineRoleStore
	{
		IUnitOfWorkFactory _unitOfWorkFactory;
		IRepository<ApplicationGroup> _groupRepository;

		public IQueryable<ApplicationGroup> Roles {
			get {
				return _groupRepository.FindAll ();
			}
		}

        IQueryable<ApplicationGroup> IQueryableRoleStore<ApplicationGroup>.Roles => throw new NotImplementedException();

        public NHibernateRoleStore (IUnitOfWorkFactory unitOfWorkFactory, IRepository<ApplicationGroup> groupRepository)
		{
			if (unitOfWorkFactory == null) {
				throw new ArgumentNullException (nameof (unitOfWorkFactory));
			}
			if (groupRepository == null) {
				throw new ArgumentNullException (nameof (groupRepository));
			}

			_unitOfWorkFactory = unitOfWorkFactory;
			_groupRepository = groupRepository;
		}

		public Task CreateAsync (ApplicationGroup role)
		{
			throw new NotImplementedException ();
		}

		public Task DeleteAsync (ApplicationGroup role)
		{
			throw new NotImplementedException ();
		}

		public void Dispose ()
		{
			
		}

		public Task<ApplicationGroup> FindByIdAsync (Guid roleId)
		{
			throw new NotImplementedException ();
		}

		public Task<ApplicationGroup> FindByNameAsync (string roleName)
		{
			throw new NotImplementedException ();
		}

		public Task UpdateAsync (ApplicationGroup role)
		{
			if (role == null) {
				throw new ArgumentNullException (nameof (role));
			}
			using (IUnitOfWork uow = _unitOfWorkFactory.BeginUnitOfWork ()) {
				_groupRepository.Add (role);
				uow.Commit ();
			}
			return Task.FromResult<object> (null);
		}

        Task<IdentityResult> IRoleStore<ApplicationGroup>.CreateAsync(ApplicationGroup role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IRoleStore<ApplicationGroup>.UpdateAsync(ApplicationGroup role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IdentityResult> IRoleStore<ApplicationGroup>.DeleteAsync(ApplicationGroup role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<ApplicationGroup>.GetRoleIdAsync(ApplicationGroup role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<ApplicationGroup>.GetRoleNameAsync(ApplicationGroup role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRoleStore<ApplicationGroup>.SetRoleNameAsync(ApplicationGroup role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<ApplicationGroup>.GetNormalizedRoleNameAsync(ApplicationGroup role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IRoleStore<ApplicationGroup>.SetNormalizedRoleNameAsync(ApplicationGroup role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationGroup> IRoleStore<ApplicationGroup>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<ApplicationGroup> IRoleStore<ApplicationGroup>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }
    }
}


using System;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity
{
	public interface IDealEngineUserStore
		: IUserStore<User>, IQueryableUserStore<User>, IUserEmailStore<User>, IUserRoleStore<User>
	{
	}
}


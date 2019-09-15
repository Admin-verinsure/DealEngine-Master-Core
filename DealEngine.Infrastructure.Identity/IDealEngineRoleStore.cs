using System;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity
{
	public interface IDealEngineRoleStore : IRoleStore<ApplicationGroup>, IQueryableRoleStore<ApplicationGroup>
	{
	}
}


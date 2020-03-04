using System;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.Ldap.Interfaces;

namespace DealEngine.Infrastructure.Ldap.Queries
{
	public class UserByIdQuery : ILdapQuery<User>
	{
		Guid _userId;

		public UserByIdQuery (Guid id)
		{
			_userId = id;
		}

		public string Compile ()
		{
			return "employeeNumber=" + _userId;
		}
	}
}


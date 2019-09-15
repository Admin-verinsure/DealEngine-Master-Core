using System;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace TechCertain.Infrastructure.Ldap.Queries
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


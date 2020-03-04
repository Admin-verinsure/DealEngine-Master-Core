using System;

namespace DealEngine.Infrastructure.BaseLdap.Entities
{
	public class LdapRole : BaseLdapEntity
	{
		public string RoleName {
			get;
			protected set;
		}

		public string Name {
			get;
			protected set;
		}

		public LdapRole (Guid id, string roleName, string name)
			: base(id)
		{
			RoleName = roleName;
			Name = name;
		}
	}
}


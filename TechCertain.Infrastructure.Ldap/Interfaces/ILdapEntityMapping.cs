using Bismuth.Ldap;
using System.Collections.Generic;

namespace TechCertain.Infrastructure.Ldap.Interfaces
{
	public interface ILdapEntityMapping<TEntity>
	{
		TEntity FromLdap (LdapEntry entry);
		string GetDn (TEntity entity, string baseDn);
		LdapEntry ToLdap (TEntity entity, string baseDn);
		List<ModifyAttribute> ToModify (TEntity entity);
	}
}


using System;

namespace DealEngine.Infrastructure.BaseLdap.Interfaces
{
	public interface IOpenLdapImportService
    {
		/// <summary>
		/// <para>Imports a user with the specified username from the legacy system to the current system.</para>
		/// <para>Throws an UserImportException if the user cannot be imported from the old Ldap server.</para>
		/// <para>Throws an Exception for any other errors.</para>
		/// </summary>
		/// <returns><c>true</c>, if user was imported, <c>false</c> otherwise.</returns>
		/// <param name="username">Username.</param>
		bool ImportUser(string username);

		/// <summary>
		/// <para>Imports a user with the specified email address from the legacy system to the current system.</para>
		/// <para>Throws an UserImportException if the user cannot be imported from the old Ldap server.</para>
		/// <para>Throws an Exception for any other errors.</para>
		/// </summary>
		/// <returns><c>true</c>, if user was imported, <c>false</c> otherwise.</returns>
		/// <param name="email">Email.</param>
		bool ImportUserByEmail(string email);

		bool ImportUserById(Guid userId);

		bool ImportOrganisation(Guid organisationId);
	}
}


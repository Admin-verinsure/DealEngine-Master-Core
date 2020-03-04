using Novell.Directory.Ldap;
using System;

namespace DealEngine.Infrastructure.Legacy.Interfaces
{
	public interface ILegacyLdapExportService
	{
		/// <summary>
		/// <para>Exports a LdapAttributeSet containing the attributes of a specified user.</para>
		/// <para>Throws an UserExportException if it is unable to export the users attributes.</para>
		/// <para>Throws an Exception for any other errors.</para>
		/// </summary>
		/// <param name="username">Username of the user to be exported.</param>
		LdapAttributeSet Export(string username);

		/// <summary>
		/// <para>Exports a LdapAttributeSet containing the attributes of a specified user.</para>
		/// <para>Throws an UserExportException if it is unable to export the users attributes.</para>
		/// <para>Throws an Exception for any other errors.</para>
		/// </summary>
		/// <param name="email">Email address of the user to be exported.</param>
		LdapAttributeSet ExportByEmail(string email);

		/// <summary>
		/// <para>Exports a LdapAttributeSet containing the attributes of a specified user.</para>
		/// <para>Throws an UserExportException if it is unable to export the users attributes.</para>
		/// <para>Throws an Exception for any other errors.</para>
		/// </summary>
		/// <param name="userId">User Id of the user to be exported.</param>
		LdapAttributeSet ExportByUserId(Guid userId);

        LdapAttributeSet Export(Guid organisationID);
	}
}
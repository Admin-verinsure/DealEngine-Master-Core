using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Domain.Interfaces
{
    public interface IUserRepository
    {
		/// <summary>
		/// <para>Gets a User with the specified username.</para>
		/// <para>Throws a System.ArgumentNullException if the userName is null, empty or whitespace.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="userName">User name.</param>
        User GetUser(string userName);

		/// <summary>
		/// <para>Gets a User with the specified username and password combination.</para>
		/// <para>Throws a System.ArgumentNullException if the userName or password is null, empty or whitespace.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="userName">User name.</param>
		/// <param name="userPassword">User password.</param>
        User GetUser(string userName, string userPassword);

		/// <summary>
		/// <para>Gets a user with the specified id.</para>
		/// <para>Throws a System.ArgumentNullException if the id is null or an empty Guid.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="userID">ID of the user.</param>
        User GetUser(Guid userID);

		/// <summary>
		/// <para>Gets a User with the specified email address.</para>
		/// <para>Throws a System.ArgumentNullException if the email is null, empty or whitespace.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="userName">User name.</param>
		User GetUserByEmail(string email);

        IEnumerable<User> GetUsers ();

		/// <summary>
		/// <para>Creates and saves the specified user in the repository.</para>
		/// <para>Throws a System.ArgumentNullException if the user is null.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <param name="user">User.</param>
        bool Create(User user);

		/// <summary>
		/// <para>Updates the specified user.</para>
		/// <para>Throws a System.ArgumentNullException if the user is null.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <param name="user">User.</param>
        bool Update(User user);

		/// <summary>
		/// <para>Deletes the specified user.</para>
		/// <para>Throws a System.ArgumentNullException if the user is null.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <param name="user">User.</param>
        bool Delete(User user);
        
    }
}

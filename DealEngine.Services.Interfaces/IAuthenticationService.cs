﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IAuthenticationService
	{
        // Anything to do with Authentication
        /// <summary>
        /// Authenticates a user with the username and password.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>Returns the User if authentication has suceeded, returns <code>null</code> if authentication has failed</returns>
        User ValidateUser (string username, string password);

		/// <summary>
		/// <para>Generates a single use token for a given email address.</para>
		/// <para>Throws a System.ArgumentNullException if the email is null, empty or whitespace.</para>
		/// <para>Throws a System.Exception for any other errors.</para>
		/// </summary>
		/// <returns>The single use token.</returns>
		/// <param name="email">Email.</param>
		Task<SingleUseToken> GenerateSingleUseToken(string email);

		SingleUseToken GetToken(Guid token);

		Task<bool> ValidSingleUseToken(Guid token);

		Task<bool> UseSingleUseToken(Guid token);

		IEnumerable<SingleUseToken> GetTokensFor (Guid userId);

		//Possibly Password Recovery options in here too
	}
}


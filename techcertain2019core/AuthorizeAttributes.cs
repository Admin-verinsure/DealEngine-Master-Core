using System;
using System.Web.Mvc;
using TechCertain.Domain.Entities;

namespace techcertain2019core
{
	/**
	 * I don't get it. Microsoft comes up with a web framework that needs DI to work correctly without a lot of boilerplate, 
	 * but then doesn't support DI on attributes without incredibly complex and convoluted workarounds.
	 * Just going to throw these into the BaseController, will have to come back later and redo these
	 **/
	public class AuthorizeRoleAttribute : FilterAttribute
	{
		public string [] Roles { get; set; }

		public AuthorizeRoleAttribute (params string[] roles)
		{
			Roles = roles;
		}
	}
}


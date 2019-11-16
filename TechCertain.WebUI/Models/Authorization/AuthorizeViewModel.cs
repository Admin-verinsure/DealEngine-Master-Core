using NHibernate.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Authorization
{
	public class AuthorizeViewModel : BaseViewModel
	{
        public IList<Claim> ClaimList { get; set; }
        public IList<IdentityRole> RoleList { get; set; }
        public IList<IdentityUser> UserList { get; set; }
    }
}


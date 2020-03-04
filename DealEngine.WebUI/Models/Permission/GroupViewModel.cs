using System;
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Identity;


namespace DealEngine.WebUI.Models.Permission
{
	public class GroupViewModel : BaseViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		////public GroupViewModel (IdentityRole group)
		////{
		////}

		public GroupViewModel (IdentityRole group)
		{
			Id = Guid.Parse(group.Id);
			Name = group.Name;
		}

	}
}


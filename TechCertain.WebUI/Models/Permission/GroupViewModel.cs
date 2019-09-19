using System;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Permission
{
	public class GroupViewModel : BaseViewModel
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public GroupViewModel ()
		{
		}

		public GroupViewModel (ApplicationGroup group)
		{
			Id = group.Id;
			Name = group.Name;
		}

		public ApplicationGroup ToEntity (User creatingUser)
		{
			return new ApplicationGroup (creatingUser, Name);
		}
	}
}


using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;

namespace DealEngine.Infrastructure.AppInitialize
{
	public class AppRolesGroupsConfigure : IAppConfigure
	{
		IUnitOfWorkFactory _uowFactory;
		IRepository<ApplicationGroup> _groupRespoitory;
		IRepository<ApplicationRole> _roleRepository;

		public AppRolesGroupsConfigure (IUnitOfWorkFactory uowFactory, IRepository<ApplicationGroup> groupRepository, IRepository<ApplicationRole> roleRepository)
		{
			_uowFactory = uowFactory;
			_groupRespoitory = groupRepository;
			_roleRepository = roleRepository;
		}

		public void Configure ()
		{
			ApplicationRole [] defaultSystemRoles = {
				new ApplicationRole(null, "Admin", "Global Access", true),
				new ApplicationRole(null, "CanEditUser", "Add, modify and delete Users", true),
				new ApplicationRole(null, "CanEditGroup", "Add, modify and delete Groups", true),
				new ApplicationRole(null, "CanEditRole", "Add, modify and delete Roles", true),
				new ApplicationRole(null, "CanEditProduct", "Add, modify and delete Products", true),
				new ApplicationRole(null, "CanEditProgramme", "Add, modify and delete Programmes", true),
				new ApplicationRole(null, "CanViewAllInformation", "Can view all available Information Sheets", true)
			};
			ApplicationGroup [] defaultSystemGroups = {
				new ApplicationGroup(null, "SystemAdmin", true),
				new ApplicationGroup(null, "BrokerAdmin", true),
				new ApplicationGroup(null, "InsurerAdmin", true),
				new ApplicationGroup(null, "Broker", true),
				new ApplicationGroup(null, "Insurer", true),
				new ApplicationGroup(null, "Client", true)
			};

			using (IUnitOfWork uow = _uowFactory.BeginUnitOfWork ()) {
				string [] groupNames = _groupRespoitory.FindAll ().Select (g => g.Name).ToArray ();
				foreach (var systemGroup in defaultSystemGroups) {
					if (!groupNames.Contains (systemGroup.Name))
						_groupRespoitory.Add (systemGroup);
					//	Console.WriteLine ("Saving group: " + systemGroup.Name);
					//else
					//	Console.WriteLine ("Skip adding group: " + systemGroup.Name);
				}

				string [] roleNames = _roleRepository.FindAll().Select (r => r.Name).ToArray ();
				foreach (var systemRole in defaultSystemRoles) {
					if (!roleNames.Contains (systemRole.Name))
						_roleRepository.Add (systemRole);
					//	Console.WriteLine ("Saving role: " + systemRole.Name);
					//else
					//	Console.WriteLine ("Skip adding role: " + systemRole.Name);
				}

				var adminGroup = _groupRespoitory.FindAll ().FirstOrDefault (g => g.Name == "SystemAdmin");
				foreach (var appRole in _roleRepository.FindAll ()) {
					if (!adminGroup.Roles.Contains (appRole))
						adminGroup.Roles.Add (appRole);
					//	Console.WriteLine ("Adding role (" + appRole.Name + ") to group: " + adminGroup.Name);
					//else
					//	Console.WriteLine ("Skip adding role (" + appRole.Name + ") to group: " + adminGroup.Name);
				}

				uow.Commit ();
			}
		}
	}
}


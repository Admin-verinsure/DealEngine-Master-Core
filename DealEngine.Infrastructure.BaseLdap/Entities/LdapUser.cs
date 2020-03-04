using System;

namespace DealEngine.Infrastructure.BaseLdap.Entities
{
	/// <summary>
	/// TC sync user is used to manage the synchronisation between the TCUser class and any external
	/// User data source.
	/// </summary>
	public class LdapUser : BaseLdapEntity
	{
		public string UserName { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public string Address { get; set; }

		public string Password { get; set; }

		public string FullName { get; set; }

		public Guid[] OrganisationIDs { get; set; }

		public Guid[] BranchIDs { get; set; }

		public Guid[] DepartmentIDs { get; set; }

		public LdapLocation Location { get; set; }

		public LdapUser (Guid userID, string strUsername)
			: base(userID)
		{
			UserName = strUsername;
			OrganisationIDs = new Guid[0];
			BranchIDs = new Guid[0];
			DepartmentIDs = new Guid[0];
		}

		//public Nullable<System.DateTime> DateActivated { get; set; }

		//public string SecretQuestion { get; set; }

		//public string SecretAnswer { get; set; }

//		public bool Activated {
//			get {
//				return DateActivated.HasValue;
//			}
//		}

		//		bool ExternalSync (TCSyncUser SourceUser);

		//public bool MustChangePassword { get; set; }

		//public Guid OrganizationID { get; set; }

		//		Might be used later
		//		public DateTime? NoOrganisation {
		//			get;
		//			set;
		//		}

		//public LdapOrganisation Organisation { get; set; }

		//public DateTime DateModified { get; set; }

		//public Guid[] OrganisationBranches{ get; set; }

		//public bool OrganisationContactUser { get; set; }
	}
}


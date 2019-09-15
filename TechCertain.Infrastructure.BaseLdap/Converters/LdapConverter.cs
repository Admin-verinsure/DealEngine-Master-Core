using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.BaseLdap.Entities;
using TechCertain.Infrastructure.BaseLdap.Helpers;

namespace TechCertain.Infrastructure.BaseLdap.Converters
{
	public class LdapConverter
	{
		/// <summary>
		/// <para>Converts an LdapEntry into a User entity.</para>
		/// <para>Throws a System.ArgumentNullException if the LdapEntry is null.</para>
		/// <para>Throws a System.NullReferenceException if the LdapEntry doesn't contain any attributes.</para>
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="ldapEntry">LDAP entry.</param>
        public static User ToUser(LdapEntry ldapEntry)
        {
			if (ldapEntry == null)
				throw new ArgumentNullException ("ldapEntry");

            LdapAttributeSet attributes = ldapEntry.getAttributeSet();
			if (attributes == null)
				throw new NullReferenceException ("LdapEntry doesn't contain any attributes.");

            string userName = LdapHelpers.GetLdapAttributeValue(attributes, "uid");
			if (string.IsNullOrWhiteSpace (userName))
				throw new Exception ("LdapEntry is missing uid");
			User user = new User(null, LdapHelpers.GetGuidValue(attributes, "employeeNumber"), userName);

            // Introduced another constructor for User for setting the Id.
			// user.ChangeId(LdapHelpers.GetGuidValue(attributes, "employeeNumber"));

            user.FirstName = LdapHelpers.GetLdapAttributeValue(attributes, "givenName");
            user.LastName = LdapHelpers.GetLdapAttributeValue(attributes, "sn");
            user.FullName = LdapHelpers.GetLdapAttributeValue(attributes, "cn");
            user.Email = LdapHelpers.GetLdapAttributeValue(attributes, "mail");
            user.Phone = LdapHelpers.GetLdapAttributeValue(attributes, "telephoneNumber");
			//user.ProfilePicture = LdapHelpers.GetLdapAttributeValue(attributes, "jpegPhoto");	// TODO - remap this
			user.Description = LdapHelpers.GetLdapAttributeValue(attributes, "description");

            Guid legacyId = Guid.Empty;
            Guid.TryParse(LdapHelpers.GetLdapAttributeValue(attributes, "employeeNumber"), out legacyId);
            user.LegacyId = legacyId;

            user.Location = ToLocation(attributes);
			// process operational attributes
			string lockedTime = LdapHelpers.GetLdapAttributeValue(attributes, "pwdAccountLockedTime");
			if (!string.IsNullOrWhiteSpace (lockedTime))
				user.Lock ();

            List<Organisation> organisationIDs = new List<Organisation>();
            foreach (string s in LdapHelpers.GetLdapAttributeValues(attributes, "o"))
                organisationIDs.Add(new Organisation(null, new Guid(s)));

            user.Organisations = organisationIDs;

            return user;
		}

		/// <summary>
		/// <para>Converts an LdapEntry into an Organisation entity.</para>
		/// <para>Throws a System.ArgumentNullException if the LdapEntry is null.</para>
		/// <para>Throws a System.NullReferenceException if the LdapEntry doesn't contain any attributes.</para>
		/// </summary>
		/// <returns>The user.</returns>
		/// <param name="ldapEntry">LDAP entry.</param>
		public static Organisation ToOrganisation(LdapEntry ldapEntry)
		{
			if (ldapEntry == null)
				throw new ArgumentNullException ("ldapEntry", "LdapEntry cannot be null.");

			LdapAttributeSet attributes = ldapEntry.getAttributeSet ();
			if (attributes == null)
				throw new NullReferenceException ("LdapEntry doesn't contain any attributes.");

			Guid id = LdapHelpers.GetGuidValue(attributes, "o");
			Organisation organisation = new Organisation (null, id);

			organisation.ChangeOrganisationName(LdapHelpers.GetLdapAttributeValue (attributes, "buildingName"));

			OrganisationType organisationType = new OrganisationType (null, LdapHelpers.GetLdapAttributeValue (attributes, "businessCategory"));

			organisation.ChangeOrganisationType (organisationType);
			organisation.Description	= LdapHelpers.GetLdapAttributeValue (attributes, "description");
			organisation.Phone			= LdapHelpers.GetLdapAttributeValue (attributes, "telephoneNumber");
			organisation.Domain			= LdapHelpers.GetLdapAttributeValue (attributes, "associatedDomain");
			organisation.Location		= ToLocation(attributes);

			return organisation;
		}

		[Obsolete]
        public static LdapUser ToUser_Old(LdapEntry ldapEntry)
		{
			if (ldapEntry == null)
				return null;

			LdapAttributeSet attributes = ldapEntry.getAttributeSet ();

			string userName = LdapHelpers.GetLdapAttributeValue(attributes, "uid");
			Guid userId = LdapHelpers.GetGuidValue(attributes, "entryuuid");
			LdapUser ldapUser = new LdapUser (userId, userName);

			ldapUser.FirstName	= LdapHelpers.GetLdapAttributeValue(attributes, "givenName");
			ldapUser.LastName	= LdapHelpers.GetLdapAttributeValue(attributes, "sn");
			ldapUser.FullName	= LdapHelpers.GetLdapAttributeValue(attributes, "cn");
			ldapUser.Email		= LdapHelpers.GetLdapAttributeValue(attributes, "mail");
			ldapUser.Phone		= LdapHelpers.GetLdapAttributeValue(attributes, "telephoneNumber");
			ldapUser.Location	= ToLocation_Old(attributes);

			List<Guid> organisationIDs = new List<Guid> ();
			foreach (string s in LdapHelpers.GetLdapAttributeValues(attributes, "o"))
				organisationIDs.Add (new Guid (s));

			List<Guid> departmentIDs = new List<Guid> ();
			foreach (string s in LdapHelpers.GetLdapAttributeValues(attributes, "departmentNumber"))
				departmentIDs.Add (new Guid (s));

			ldapUser.OrganisationIDs = organisationIDs.ToArray ();
			ldapUser.DepartmentIDs = departmentIDs.ToArray ();

			return ldapUser;
		}

		[Obsolete]
		public static LdapOrganisation ToOrganisation_Old(LdapEntry ldapEntry)
		{
			if (ldapEntry == null)
				return null;

			LdapAttributeSet attributes = ldapEntry.getAttributeSet ();

			Guid id = LdapHelpers.GetGuidValue(attributes, "o");
			LdapOrganisation organisation = new LdapOrganisation (id);

			organisation.Name			= LdapHelpers.GetLdapAttributeValue (attributes, "buildingName");
			organisation.Role			= LdapHelpers.GetLdapAttributeValue (attributes, "businessCategory");
			organisation.Description	= LdapHelpers.GetLdapAttributeValue (attributes, "description");
			organisation.Phone			= LdapHelpers.GetLdapAttributeValue (attributes, "telephoneNumber");
			organisation.Domain			= LdapHelpers.GetLdapAttributeValue (attributes, "associatedDomain");
			organisation.Location		= ToLocation_Old(attributes);

			return organisation;
		}

		[Obsolete]
		public static LdapRole ToRole(LdapEntry ldapEntry)
		{
			if (ldapEntry == null)
				return null;

			LdapAttributeSet attributes = ldapEntry.getAttributeSet ();

			Guid id = LdapHelpers.GetGuidValue(attributes, "entryuuid");
			string roleName = LdapHelpers.GetLdapAttributeValue (attributes, "cn");
			string name = LdapHelpers.GetLdapAttributeValue (attributes, "ou");

			LdapRole role = new LdapRole (id, roleName, name);

			return role;
		}

		[Obsolete]
		public static LdapBranch ToBranch(LdapEntry ldapEntry)
		{
			if (ldapEntry == null)
				return null;

			LdapAttributeSet attributes = ldapEntry.getAttributeSet ();

			Guid id = LdapHelpers.GetGuidValue(attributes, "entryuuid");
			Guid organisationID = LdapHelpers.GetGuidValue(attributes, "ou");
			string branchName = LdapHelpers.GetLdapAttributeValue (attributes, "buildingName");
			LdapBranch branch = new LdapBranch (id, organisationID, branchName);

			branch.BranchLocation = ToLocation_Old (attributes);
			
			return branch;
		}

		[Obsolete]
		public static LdapDepartment ToDepartment(LdapEntry ldapEntry)
		{
			if (ldapEntry == null)
				return null;
			
			return null;
		}

		/// <summary>
		/// <para>Converts a LdapAttributeSet to a Location Entity.</para>
		/// <para>Throws a System.ArgumentNullException if its LdapEntry is null.</para>
		/// </summary>
		/// <returns>The location.</returns>
		/// <param name="attributes">Attributes.</param>
        public static Location ToLocation(LdapAttributeSet attributes)
        {
			if (attributes == null)
				throw new ArgumentNullException ("attributes", "LdapAttributeSet cannot be null.");

            Location location = new Location(null);
            location.Street = LdapHelpers.GetLdapAttributeValue(attributes, "street");
            location.Postcode = LdapHelpers.GetLdapAttributeValue(attributes, "postCode");
            location.City = LdapHelpers.GetLdapAttributeValue(attributes, "l");
            location.State = LdapHelpers.GetLdapAttributeValue(attributes, "st");
            location.Country = LdapHelpers.GetLdapAttributeValue(attributes, "c");

            return location;
        }

		[Obsolete]
        public static LdapLocation ToLocation_Old(LdapAttributeSet attributes)
		{
			if (attributes == null)
				return null;
				
			LdapLocation location = new LdapLocation ();
			location.Street		= LdapHelpers.GetLdapAttributeValue (attributes, "street");
			location.Postcode	= LdapHelpers.GetLdapAttributeValue (attributes, "postCode");
			location.City		= LdapHelpers.GetLdapAttributeValue (attributes, "l");
			location.State		= LdapHelpers.GetLdapAttributeValue (attributes, "st");
			location.Country	= LdapHelpers.GetLdapAttributeValue (attributes, "c");

			return location;
		}

        public static LdapEntry ToEntry(User user, string dn)
        {
			LdapAttributeSet attrs = new LdapAttributeSet ();
			attrs.AddAttribute ("objectclass", new string[] { "top", "person", "organizationalPerson", "inetOrgPerson" });
			attrs.AddAttribute ("uid", user.UserName);
			attrs.AddAttribute ("employeeNumber", user.Id.ToString());
			attrs.AddAttribute ("givenName", user.FirstName);
			attrs.AddAttribute ("sn", user.LastName);
			attrs.AddAttribute ("cn", user.FullName);
			attrs.AddAttribute ("mail", user.Email);
			if (!string.IsNullOrWhiteSpace (user.Phone))
				attrs.AddAttribute ("telephoneNumber", user.Phone);
			attrs.AddAttribute ("telephoneNumber", user.Phone);
			attrs.AddAttribute ("userPassword", user.Password);
			attrs.AddAttribute ("o", user.Organisations.Select(o => o.Id.ToString()).ToArray());

			LdapEntry entry = new LdapEntry (dn, attrs);

			return entry;
        }

        public static LdapEntry ToEntry(Organisation organisation, string dn)
        {
            LdapAttributeSet attrs = new LdapAttributeSet();
            attrs.AddAttribute("objectclass", new string[] { "top", "pilotOrganization", "domainRelatedObject" });
            attrs.AddAttribute("buildingName", organisation.Name);
            if (organisation.OrganisationType != null)
                attrs.AddAttribute("businessCategory", organisation.OrganisationType.Name);
			if (!string.IsNullOrWhiteSpace(organisation.Description))
            	attrs.AddAttribute("description", organisation.Description);
			if (!string.IsNullOrWhiteSpace (organisation.Phone))
            	attrs.AddAttribute("telephoneNumber", organisation.Phone);
			// need to have associatedDomain
			if (!string.IsNullOrWhiteSpace (organisation.Domain))
            	attrs.AddAttribute("associatedDomain", organisation.Domain);
			else
				attrs.AddAttribute ("associatedDomain", "#");
            attrs.AddAttribute("o", organisation.Id.ToString());
			attrs.AddAttribute("ou", "organisation");

            LdapEntry entry = new LdapEntry(dn, attrs);

            return entry;
        }

        [Obsolete]
        public static LdapEntry ToEntry(LdapUser user, string dn)
		{
			return null;
		}

		[Obsolete]
		public static LdapEntry ToEntry(LdapOrganisation organisation, string dn)
		{
			return null;
		}

		/// <summary>
		/// <para>Converts a User Entity to an LdapModification array.</para>
		/// <para>Throws a System.ArgumentNullException if the User is null.</para>
		/// </summary>
		/// <returns>The modification array.</returns>
		/// <param name="user">User.</param>
        public static LdapModification[] ToModificationArray(User user)
		{
			if (user == null)
				throw new ArgumentNullException ("user", "User cannot be null.");
			
			try
			{
	            var departmentIDs = user.Departments.Select(d => d.Id).ToArray();
	            var organisationIDs = user.Organisations.Select(d => d.Id).ToArray();
	            var branchIDs = user.Branches.Select(d => d.Id).ToArray();

	            List<LdapModification> mods = new List<LdapModification>()
	            {
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "givenname", user.FirstName),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "sn", user.LastName),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "mail", user.Email),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "telephonenumber", user.Phone),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "street", user.Address),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "o", organisationIDs),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "ou", branchIDs),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "departmentNumber", departmentIDs),
	                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "uid", user.UserName),
					LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "userPassword", user.Password),
					//LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "jpegPhoto", user.ProfilePicture),	TODO - remap this
					LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "description", user.Description)
	            };
	            if (user.Location != null)
	                mods.AddRange(ToModificationArray(user.Location));
	            return mods.Where(m => m != null).ToArray();
			}
			catch (Exception ex)
			{
				throw new Exception ("Unable to convert User to LdapModification array.", ex);
				//Console.WriteLine (e.ToString ());
			}
        }

		[Obsolete]
        public static LdapModification[] ToModificationArray(LdapUser user)
		{
			List<LdapModification> mods = new List<LdapModification> ()
			{
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "givenname", user.FirstName),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "sn", user.LastName),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "mail", user.Email),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "telephonenumber", user.Phone),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "street", user.Address),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "o", user.OrganisationIDs),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "ou", user.BranchIDs),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "departmentNumber", user.DepartmentIDs),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "uid", user.UserName),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "userPassword", user.Password)
			};
			if (user.Location != null)
				mods.AddRange (ToModificationArray (user.Location));
			return mods.Where (m => m != null).ToArray();
		}

		[Obsolete]
		public static LdapModification[] ToModificationArray(LdapOrganisation organisation)
		{
			List<LdapModification> mods = new List<LdapModification> ()
			{
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "buildingName", organisation.Name),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "businessCategory", organisation.Role),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "description", organisation.Description),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "telephoneNumber", organisation.Phone),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "associatedDomain", organisation.Domain)
			};
			if (organisation.Location != null)
				mods.AddRange (ToModificationArray (organisation.Location));
			return mods.Where (m => m != null).ToArray();
		}

		[Obsolete]
		public static LdapModification[] ToModificationArray(LdapBranch branch)
		{
			List<LdapModification> mods = new List<LdapModification> ()
			{
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "buildingName", branch.Name),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "ou", branch.OrganisationID)
			};
			if (branch.BranchLocation != null)
				mods.AddRange (ToModificationArray (branch.BranchLocation));
			return mods.Where (m => m != null).ToArray();
		}

		[Obsolete]
		public static LdapModification[] ToModificationArray(LdapDepartment department)
		{
			List<LdapModification> mods = new List<LdapModification> ()
			{
				//TODO
			};
//			if (department.Location != null)
//				mods.AddRange (ToModificationArray (department.Location));
			return mods.Where (m => m != null).ToArray();
		}

		/// <summary>
		/// <para>Converts a Location Entity to an LdapModification array.</para>
		/// <para>Throws a System.ArgumentNullException if the Location is null.</para>
		/// </summary>
		/// <returns>The modification array.</returns>
		/// <param name="location">Location.</param>
        public static LdapModification[] ToModificationArray(Location location)
		{
			if (location == null)
				throw new ArgumentNullException ("location", "Location cannot be null.");
			
			try
			{
            List<LdapModification> mods = new List<LdapModification>()
            {
                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "street", location.Street),
                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "l", location.City),
                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "postalCode", location.Postcode),
                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "st", location.State),
                LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "c", location.Country)
				};
				return mods.Where(m => m != null).ToArray();
			}
			catch (Exception e)
			{
				throw new Exception ("Unable to convert Location to LdapModification array.", e);
				//Console.WriteLine (e.ToString ());
			}
        }

		[Obsolete]
        public static LdapModification[] ToModificationArray(LdapLocation location)
		{
			List<LdapModification> mods = new List<LdapModification> ()
			{
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "street", location.Street),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "l", location.City),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "postalCode", location.Postcode),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "st", location.State),
				LdapHelpers.GetLdapModifider (LdapModification.REPLACE, "c", location.Country)
			};

			return mods.Where (m => m != null).ToArray();
		}
	}
}


﻿using Bismuth.Ldap;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.Ldap.Interfaces;
using System.Linq;

namespace TechCertain.Infrastructure.Ldap.Mapping
{
	public class UserMapping : BaseEntityMapping, ILdapEntityMapping<User>
	{
		public User FromLdap (LdapEntry entry)
		{
			Guid id = Guid.Parse (entry.GetAttributeValue ("employeeNumber"));                  // Need to swap this to use 'uniqueIdentifier'
			string userName = entry.GetAttributeValue ("uid");

			User user = new User (null, id, userName);
			user.FirstName = entry.GetAttributeValue ("givenName");
			user.LastName = entry.GetAttributeValue ("sn");
			user.FullName = entry.GetAttributeValue ("cn");
			user.Email = entry.GetAttributeValue ("mail");
			user.Phone = entry.GetAttributeValue ("telephoneNumber");
			user.Description = entry.GetAttributeValue ("description");

			return user;
		}

		public string GetDn (User entity, string baseDn)
		{
			return "uid=" + entity.UserName + ",ou=users," + baseDn;
		}

		public LdapEntry ToLdap (User entity, string baseDn)
		{
			List<ObjectAttribute> attributes = new List<ObjectAttribute> ();
			attributes.Add (new ObjectAttribute ("uid", entity.UserName));

			LdapEntry entry = new LdapEntry (GetDn (entity, baseDn))
				.AddAttribute ("uid", entity.UserName)
				.AddAttribute ("objectClass", "top", "person", "organizationalPerson", "inetOrgPerson")
				.AddAttribute ("employeeNumber", entity.Id.ToString ());
				//.AddAttribute ("uniqueIdentifier", entity.Id.ToString ());		// Removed in RFC4524 schema
			AddNonNullAttribute (entry, "mail", entity.Email);
			AddNonNullAttribute (entry, "sn", entity.LastName);
			AddNonNullAttribute (entry, "givenName", entity.FirstName);
			AddNonNullAttribute (entry, "cn", entity.FullName);
			AddNonNullAttribute (entry, "telephoneNumber", entity.Phone);
			AddNonNullAttribute (entry, "description", entity.Description);
			// TODO - add org Id map here
			entry.AddAttribute ("o", entity.Organisations.Select (o => o.Id.ToString ()).ToArray());
			
			return entry;
		}

		public List<ModifyAttribute> ToModify (User entity)
		{
			var userMods = new List<ModifyAttribute> ();
			AddNonNullAttribute (userMods, "mail", ModificationType.Replace, entity.Email);
			AddNonNullAttribute (userMods, "givenName", ModificationType.Replace, entity.FirstName);
			AddNonNullAttribute (userMods, "sn", ModificationType.Replace, entity.LastName);
			AddNonNullAttribute (userMods, "cn", ModificationType.Replace, entity.FullName);
			AddNonNullAttribute (userMods, "telephoneNumber", ModificationType.Replace, entity.Phone);
			AddNonNullAttribute (userMods, "description", ModificationType.Replace, entity.Description);
			//AddNonNullAttribute (userMods, "uniqueIdentifier", ModificationType.Replace, entity.Id.ToString ());		// Removed in RFC4524 schema
			// TODO - add org Id map here
			userMods.Add (new ModifyAttribute ("o", ModificationType.Replace, entity.Organisations.Select (o => o.Id.ToString ()).ToArray ()));

			return userMods;
		}
	}
}


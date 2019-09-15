using System;
using System.Collections.Generic;
using System.Text;

namespace TechCertain.Infrastructure.BaseLdap.Entities
{
	public class LdapInstance : BaseLdapEntity
	{
		//[LdapAttribute ("tcinstancename")]
		public string Name { get ; set; }

		//[LdapAttribute ("tcinstanceurl")]
		public string URL { get ; set; }

		public string LogoURL { get ; set ; }

		public string ContactPageContent { get; set; }

		public bool HasContactDetails {
			get {
				return !String.IsNullOrEmpty (ContactPageContent);
			}
		}

		public LdapInstance (Guid instanceID, string instanceName, string instanceURL, 
			string instanceLogoURL = "", string contactPageContent = "")
			: base(instanceID)
		{
			this.Name = instanceName;
			this.URL = instanceURL;
			this.LogoURL = instanceLogoURL;
			this.ContactPageContent = contactPageContent;
		}

		//		public void SetContactPageContent (String strContent)
		//		{
		//			if (TC_LDAP.Instance_SetContactPageContent (this.URL, strContent, string.IsNullOrEmpty (this.ContactPageContent)))
		//				ContactPageContent = strContent;
		//
		//		}

		public class TCInstanceCollection : List<LdapInstance>
		{
			public bool Contains (Guid guidInstanceID)
			{
				foreach (LdapInstance objInstance in this)
					if (objInstance.ID == guidInstanceID)
						return true;
				return false;
			}

			public virtual new bool Contains (LdapInstance objInstance)
			{
				foreach (LdapInstance colInstance in this)
					if (colInstance.ID == objInstance.ID)
						return true;
				return false;
			}

			public override string ToString ()
			{
				var sb = new StringBuilder ();
				foreach (LdapInstance objInstance in this) {
					sb.Append (objInstance.Name);
					sb.Append (", ");
				}
				return sb.ToString ().TrimEnd (',', ' ');
			}
		}
	}
}


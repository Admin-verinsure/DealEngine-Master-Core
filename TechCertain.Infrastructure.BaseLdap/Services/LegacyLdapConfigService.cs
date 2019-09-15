using System;
using System.Configuration;
using TechCertain.Infrastructure.Legacy.Interfaces;

namespace TechCertain.Infrastructure.Legacy.Services
{
	public class LegacyLdapConfigService : ILegacyLdapConfigService
    {
        #region Ldap Configuration

        public string BaseDN
        {
            get {
                return GetConfigValue("LegacyLdapBaseDN");
            }
        }

        public string BaseOrganisationDN
        {
            get {
                return string.Format(GetConfigValue("LegacyLdapOrganisationDN"), BaseDN);
            }
		}

		public string[] SearchAttributes {
			get {
				return new string[] { "*", "+" };
			}
		}

        public string ServerAddress {
			get {
				return GetConfigValue("LegacyLdapServer");
			}
		}

		public int ServerPort {
			get {
				return Convert.ToInt32(GetConfigValue("LegacyLdapPort"));
			}
		}

		public string AdminBindDN {
			get {
				return GetConfigValue("LegacyLdapBindDN");
			}
		}

		public string AdminBindPassword {
			get {
				return GetConfigValue("LegacyLdapBindPW");
			}
		}

		public string BaseUserDN
		{
			get {
				return string.Format(GetConfigValue("LegacyLdapBaseUserDN"), BaseDN);
			}
		}

		public string GetUserDN (string userName)
		{
			return string.Format (GetConfigValue ("LegacyLdapUserDNFromUsername"), userName);
		}

		public string GetUsersByIdSearch(Guid userID)
		{
			string s = GetConfigValue("LegacyLdapSearchUserByID");
			return string.Format (s, userID.ToString());
		}

		public string GetUsersByEmailSearch(string email)
		{
			string s = GetConfigValue("LegacyLdapSearchUserByEmail");
			return string.Format (s, email);
		}

        #endregion

        #region Lagacy Ldap Configuration

        public string[] OrganisationAttributes
        {
            get
            {                
                return GetConfigValue("LegacyLdapOrganisationAttributes").Split(',');
            }
        }
        
        public string GetOrganisationSearchDN(Guid organisationID)
        {
            return string.Format(GetConfigValue("LegacyLdapSearchOrganisationByID"), organisationID.ToString());
        }

        #endregion
        

        #region Not implemented Methods

        public string BasePoliciesDN
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string BaseBranchDN
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string BaseDepartmentDN
        {
            get
            {
                throw new NotImplementedException();
            }
		}

		public string BaseRoleDN
		{
			get
			{
				throw new NotImplementedException();
			}
		}
        
        protected string GetConfigValue(string key)
		{
			return ConfigurationManager.AppSettings [key];
		}

        public string GetOrganisationDN(Guid organisationID)
        {
            throw new NotImplementedException();
        }

        public string GetDepartmentDN(Guid departmentID)
        {
            throw new NotImplementedException();
        }

        public string GetBranchDN(Guid branchId)
        {
            throw new NotImplementedException();
		}

		public string GetRoleDN (string roleName)
		{
			throw new NotImplementedException();
		}

        public string GetBranchesDNByOrganisation(Guid organisationID)
        {
            throw new NotImplementedException();
        }

        public string GetDepartmentsDNByOrganisation(Guid organisationID)
        {
            throw new NotImplementedException();
        }

        public string GetUsersByOrganisationSearch(Guid organisationID)
        {
            throw new NotImplementedException();
        }

        public string GetUsersByBranchSearch(Guid branchID)
        {
            throw new NotImplementedException();
        }

        public string GetUsersByDepartmentSearch(Guid department)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}


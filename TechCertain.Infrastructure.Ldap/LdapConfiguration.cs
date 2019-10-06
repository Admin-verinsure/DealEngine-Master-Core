﻿using Microsoft.Extensions.Configuration;
using System;
using TechCertain.Infrastructure.Ldap.Interfaces;

namespace TechCertain.Infrastructure.Ldap
{
	public class LdapConfiguration : ILdapConfiguration
	{
        private IConfiguration _configuration { get; set; }
        public LdapConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
		public string AdminDn {
			get {
                return (_configuration.GetValue<string>("OpenLdapBindDN") + BaseDn);
                //return string.Format (ConfigurationManager.AppSettings ["OpenLdapBindDN"], BaseDn);
			}
		}

		public string AdminPassword {
			get {
                return _configuration.GetValue<string>("OpenLdapBindPW");
                //return ConfigurationManager.AppSettings ["OpenLdapBindPW"];
            }
		}

		public string LdapHost {
			get {
                return _configuration.GetValue<string>("OpenLdapServer");
                //return ConfigurationManager.AppSettings ["OpenLdapServer"];
			}
		}

		public int LdapPort {
			get {
                return _configuration.GetValue<int>("OpenLdapPort");
                //return int.Parse (ConfigurationManager.AppSettings ["OpenLdapPort"]);
			}
		}

		public string BaseDn {
			get {
                return _configuration.GetValue<string>("OpenLdapBaseDN");
                //return ConfigurationManager.AppSettings ["OpenLdapBaseDN"];
			}
		}
	}
}


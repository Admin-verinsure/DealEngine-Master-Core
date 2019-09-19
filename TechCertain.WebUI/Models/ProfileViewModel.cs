using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models
{
	public class ProfileViewModel : BaseViewModel
	{
		public string FirstName {
			get;
			set;
		}

		public string LastName {
			get;
			set;
		}

		public string Email {
			get;
			set;
		}

		public string Phone {
			get;
			set;
		}

		public string PrimaryOrganisationName {
			get;
			set;
		}

		public string Title {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		public bool ViewingSelf {
			get;
			set;
		}

		public string ProfilePicture {
			get;
			set;
   		}

        public User CurrentUser
        {
            get;
            set;
        }

        public string SalesPersonUserName
        {
            get;
            set;
        }

        public string EmployeeNumber
        {
            get;
            set;
        }

        public string JobTitle
        {
            get;
            set;
        }

        public List<OrganisationalUnitViewModel> OrganisationalUnitsVM { get; set; }

        public OrganisationalUnit DefaultOU
        {
            get;
            set;
        }
    }

}


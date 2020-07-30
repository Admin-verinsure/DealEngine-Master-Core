﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DealEngine.Domain.Entities
{
    public class User : EntityBase, IAggregateRoot
    {
        [JsonIgnore]
        private Organisation _primaryOrganisation;

        protected User() : this(null) { }

        protected User (User createdBy)
			: base (null)
		{
			Organisations = new List<Organisation> ();
			Branches = new List<OrganisationalUnit> ();
			Departments = new List<Department> ();
            UISIssueNotifyProgrammes = new List<Programme>();
            UISSubmissionNotifyProgrammes = new List<Programme>();
            AgreementReferNotifyProgrammes = new List<Programme>();
            AgreementIssueNotifyProgrammes = new List<Programme>();
            AgreementBoundNotifyProgrammes = new List<Programme>();
            PaymentConfigNotifyProgrammes = new List<Programme>();
            InvoiceConfigNotifyProgrammes = new List<Programme>();
        }

        [JsonIgnore]
        public virtual OrganisationalUnit DefaultOU { get; set; }
        public virtual string UserName { get; set; }
        public virtual string SalesPersonUserName { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual string EmployeeNumber { get; set; }
        [Display(Name = "First Name")]
        public virtual string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public virtual string LastName { get; set; }

        public virtual string Email { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Address { get; set; }

        public virtual string Password { get; set; }

        public virtual string FullName { get; set; }

		public virtual string Description { get; set; }
        [JsonIgnore]
        public virtual Image ProfilePicture { get; set; }

		public virtual bool Locked { get; protected set; }
        [JsonIgnore]
        public virtual Guid LegacyId { get; set; }
        [JsonIgnore]
        public virtual DateTime? LockTime { get; protected set; }

        public virtual string MobilePhone { get; set; }

        [Display(Name = "Date of Birth")]
        public virtual DateTime? DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the users primary organisation.
        /// The primary organisation is defined as the users current organisation for the purposes of tracking organisation ownership and related permissions.
        /// If no organisation is specifically set, it will use the 1st organisation the user belongs to as the primary unless overridden. If the user belongs to no organisations 
        /// (something that should be impossible due to individual user organisations) it will return null.
        /// </summary>
        /// <value>The primary organisation.</value>
        [JsonIgnore]
        public virtual Organisation PrimaryOrganisation { 
			get {
				if (_primaryOrganisation != null)
					return _primaryOrganisation;
				else if (Organisations != null && Organisations.Count > 0) {
					_primaryOrganisation = Organisations [0];
					return _primaryOrganisation;
				} else
					return null;
			}
			protected set {
				_primaryOrganisation = value;
			}
		}

        //public virtual IEnumerable<Organisation> Organisations { get; set; }
        [JsonIgnore]
        public virtual IList<Organisation> Organisations { get; set; }

        //public Guid[] OrganisationIDs { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<OrganisationalUnit> Branches { get; set; }

        //public Guid[] BranchIDs { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Department> Departments { get; set; }

        //public Guid[] DepartmentIDs { get; set; }
        [JsonIgnore]
        public virtual Location Location { get; set; }

        //public virtual UserTask LastActiveTask { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> UISIssueNotifyProgrammes { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> UISSubmissionNotifyProgrammes { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> AgreementReferNotifyProgrammes { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> AgreementIssueNotifyProgrammes { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> AgreementBoundNotifyProgrammes { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> PaymentConfigNotifyProgrammes { get; set; }
        [JsonIgnore]
        public virtual IList<Programme> InvoiceConfigNotifyProgrammes { get; set; }
        

        public User(User createdBy, string strUsername)
			: this (createdBy)
        {
            UserName = strUsername;

        }

        public User(User createdBy, Guid id, string userName) : this(createdBy, userName)
        {
            Id = id;
        }
        public User(User createdBy, Guid id)
            : this(createdBy)
        {
            Id = id;
        }

        public User(User createdBy, Guid guid, IFormCollection collection)
            :this(createdBy, guid)
        {
            if (collection.Any())
            {
                PopulateEntity(collection);
                Random random = new Random();
                UserName = FirstName.Replace(" ", string.Empty)
                    + "_"
                    + LastName.Replace(" ", string.Empty)
                    + random.Next(1000);
                FullName = FirstName + " " + LastName;
            }
        }

        public virtual void Lock()
		{
			Locked = true;
			LockTime = DateTime.UtcNow;
		}

		public virtual void Unlock()
		{
			Locked = false;
			LockTime = null;
		}

		//public virtual Organisation PrimaryOrganisation()
		//{
		//	IList<Organisation> orgs = null;
		//	if ((orgs = Organisations as IList<Organisation>) != null && orgs.Count > 0)
		//		return orgs [0];
		//	return null;
		//}

		public virtual void SetPrimaryOrganisation (Organisation organisation)
		{
			if (organisation == null)
				throw new ArgumentNullException (nameof (organisation));

			PrimaryOrganisation = organisation;
		}

		public virtual Organisation GetPersonalOrganisation()
		{
			foreach (Organisation org in Organisations)
				if (org.OrganisationType != null && org.OrganisationType.Name == "personal")
					return org;
			// we shouldn't get here since every user should have a personal organisation, but if we do, throw an exception since we're not supposed to get here
			throw new Exception (string.Format ("User {0} doesn't have a personal organisation", UserName));
		}
    }

    public class Location : EntityBase, IAggregateRoot
    {
		protected Location () : base (null) { }

		public Location (User createdBy) : base (createdBy) {
            OrganisationalUnits = new List<OrganisationalUnit>();
        }

		public virtual Location OriginalLocation { get; protected set; }

		public virtual ClientInformationSheet ClientInformationSheet { get; set; }

        public virtual string Street { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Country { get; set; }
        public virtual string CommonName { get; set; }
        public virtual string Street2 { get; set; }
        public virtual string AreaUnit { get; set; }
        public virtual string CNARID { get; set; }
        public virtual string DPID { get; set; }
        public virtual int GeocodeReliability { get; set; }
        public virtual decimal Latitude { get; set; }
        public virtual decimal Longitude { get; set; }
        public virtual string MeshBlockID { get; set; }
        public virtual string ValidationStatus { get; set; }
        public virtual bool Removed { get; set; }
        public virtual string RiskZone { get; set; }
        public virtual IList<OrganisationalUnit> OrganisationalUnits { get; set; }
        public virtual IList<Building> Buildings { get; set; }
        public virtual IList<WaterLocation> WaterLocations { get; set; }
        public virtual string LocationType { get; set; }

        public virtual Location CloneForNewSheet (ClientInformationSheet newSheet)
		{
			if (ClientInformationSheet == newSheet)
				throw new Exception ("Cannot clone location for original information");

			Location newLocation = new Location (newSheet.CreatedBy);
			newLocation.Street = Street;
			newLocation.Suburb = Suburb;
			newLocation.Postcode = Postcode;
			newLocation.City = City;
			newLocation.State = State;
			newLocation.Country = Country;
			newLocation.CommonName = CommonName;
			newLocation.OrganisationalUnits = new List<OrganisationalUnit> (OrganisationalUnits);
            newLocation.LocationType = LocationType;

            newLocation.OriginalLocation = this;
			return newLocation;
		}
    }

    public class Owner : User
    {
    }

}


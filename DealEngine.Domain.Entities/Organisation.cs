﻿

using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;

namespace DealEngine.Domain.Entities
{
    public class Organisation : EntityBase, IAggregateRoot
    {
        string _name;
        string _phone;
        string _SkipperExp;
        OrganisationType _organisationType;

        #region Constructors
        protected Organisation() : base(null) { }

        protected Organisation(User createdBy)
            : base(createdBy)
        {
            OrganisationalUnits = new List<OrganisationalUnit>();
            Programmes = new List<Programme>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            Marinaorgmooredtype = new List<string>();
        }

        public Organisation(User createdBy, Guid id)
            : base(createdBy)
        {
            Id = id;
            OrganisationalUnits = new List<OrganisationalUnit>();
            Programmes = new List<Programme>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            Marinaorgmooredtype = new List<string>();
        }

        public Organisation(User createdBy, string organisationName)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
                throw new ArgumentNullException(nameof(organisationName), "Not allowed to create an organisation with no name.");

            _name = organisationName;
            OrganisationalUnits = new List<OrganisationalUnit>();
            Programmes = new List<Programme>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            Marinaorgmooredtype = new List<string>();
        }

        public Organisation(User createdBy, Guid id, string organisationName, OrganisationType organisationType)
            : this(createdBy, organisationName, organisationType)
        {
            Id = id;
        }

        public Organisation(User createdBy, Guid id, string organisationName)
            : this(createdBy, organisationName)
        {
            Id = id;
        }

        public Organisation(User createdBy, string organisationName, OrganisationType organisationType)
            : this(createdBy, organisationName)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType), "Not allowed to create an organisation without specifying a type.");

            _name = organisationName;
            _organisationType = organisationType;
        }

        public Organisation(User createdBy, string organisationName, OrganisationType organisationType, string email, string Phone)
           : this(createdBy, organisationName)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType), "Not allowed to create an organisation without specifying a type.");

            _name = organisationName;
            Email = email;
            _phone = Phone;
            _organisationType = organisationType;
        }

        public Organisation(User createdBy, Guid id, string organisationName, OrganisationType organisationType, string email)
          : this(createdBy, organisationName, organisationType)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType), "Not allowed to create an organisation without specifying a type.");
            Id = id;
            Email = email;
        }

        #endregion

        #region Getters
        public virtual string Name
        {
            get { return _name; }
        }

        public virtual string SkipperExp
        {
            get;
            set;
        }


        public virtual string Website
        {
            get;
            set;
        }
        public virtual DateTime DateofRetirement
        {
            get;
            set;
        }

        public virtual bool IsRetiredorDecieved
        {
            get;
            set;
        }
        public virtual string Activities
        {
            get;
            set;
        }


        public virtual DateTime DateofDeceased
        {
            get;
            set;
        }

        public virtual DateTime DateofBirth
        {
            get;
            set;
        }

        public virtual IList<Boat> Boat
        {
            get;
            set;
        }

        public virtual IList<string> Marinaorgmooredtype
        {
            get;
            set;
        }

        public virtual OrganisationType OrganisationType
        {
            get { return _organisationType; }
        }
        public virtual bool Removed
        {
            get;
            set;
        }
        public virtual bool IsCurrentMembership { get; set; }
       // public virtual string Othercompanyname { get; set; }
        public virtual string DateQualified { get; set; }
        public virtual string DesignLicensed { get; set; }
        public virtual string SiteLicensed { get; set; }
        public virtual bool IsRegisteredLicensed { get; set; }

        public virtual Location Location { get; set; }
        public virtual string Description { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Domain { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsBroker { get; set; }
        public virtual bool IsInsurer { get; set; }
        public virtual bool IsReinsurer { get; set; }
        public virtual bool IsTC { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual string Qualifications { get; set; }
        public virtual bool IsNZIAmember { get; set; }
        public virtual string NZIAmembership { get; set; }
        public virtual string CPEngQualified { get; set; }
        public virtual bool IsADNZmember { get; set; }
        public virtual string IsIPENZmember { get; set; }
        public virtual string YearofPractice { get; set; }
        public virtual string PrevPractice { get; set; }
        public virtual string Type { get; set; }
        public virtual string PMICert { get; set; }
        public virtual string CertType { get; set; }
        public virtual bool IsLPBCategory3 { get; set; }
        public virtual bool MajorShareHolder { get; set; }
        public virtual bool IsContractorInsured { get; set; }
        public virtual bool IsInsuredRequired { get; set; }
        public virtual bool IsOtherdirectorship { get; set; }
        public virtual string InsuredEntityRelation { get; set; }
        public virtual string OtherCompanyname { get; set; }
        public virtual bool IsAffiliation { get; set; }
        public virtual string AffiliationDetails { get; set; }
        public virtual string ProfAffiliation { get; set; }
        public virtual string JobTitle { get; set; }
        public virtual string PartyName { get; set; }
        public virtual string CurrentMembershipNo { get; set; }
        public virtual IList<Programme> Programmes { get; set; }
        public virtual IList<InsuranceAttribute> InsuranceAttributes { get; set; }

        #endregion

        #region Opperations

        public virtual void ChangeOrganisationType(OrganisationType organisationType)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType));

            _organisationType = organisationType;
        }

        public virtual void ChangeOrganisationName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            _name = name;
        }
        #endregion

        public virtual IList<OrganisationalUnit> OrganisationalUnits { get; set; }
        public virtual string RegisteredStatus { get; set; }
        public virtual string Duration { get; set; }

        public static Organisation CreateDefaultOrganisation(User creatingUser, User owner, OrganisationType organisationType)
        {
            return new Organisation(creatingUser, Guid.NewGuid(), "Default user organisation for " + owner.FullName, organisationType)
            {
                Domain = "#"
            };
        }

    }
}
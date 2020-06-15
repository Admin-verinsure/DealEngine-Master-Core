﻿

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Domain.Entities
{
    public class Organisation : EntityBase, IAggregateRoot
    {
        #region Constructors
        public Organisation() : base(null) { }

        protected Organisation(User createdBy)
            : base(createdBy)
        {
            OrganisationalUnits = new List<OrganisationalUnit>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            Marinaorgmooredtype = new List<string>();
        }

        public Organisation(User createdBy, Guid id)
            : base(createdBy)
        {
            Id = id;
            OrganisationalUnits = new List<OrganisationalUnit>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            Marinaorgmooredtype = new List<string>();
        }

        public Organisation(User createdBy, string organisationName)
            : base(createdBy)
        {
            if (string.IsNullOrWhiteSpace(organisationName))
                throw new ArgumentNullException(nameof(organisationName), "Not allowed to create an organisation with no name.");

            Name = organisationName;
            OrganisationalUnits = new List<OrganisationalUnit>();
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

            Name = organisationName;
            OrganisationType = organisationType;
        }

        public Organisation(User createdBy, string organisationName, OrganisationType organisationType, string email, string phone)
           : this(createdBy, organisationName)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType), "Not allowed to create an organisation without specifying a type.");

            Name = organisationName;
            Email = email;
            Phone = phone;
            OrganisationType = organisationType;
        }

        public Organisation(User createdBy, Guid id, string organisationName, OrganisationType organisationType, string email)
          : this(createdBy, organisationName, organisationType)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType), "Not allowed to create an organisation without specifying a type.");
            Id = id;
            Email = email;
        }

        public Organisation(User creator, Guid id, string organisationName, OrganisationType organisationType, OrganisationalUnit organisationalUnit, InsuranceAttribute insuranceAttribute)
            :this(creator, organisationName, organisationType)
        {
            Name = organisationName;
            Id = id;
            OrganisationalUnits.Add(organisationalUnit);
            InsuranceAttributes.Add(insuranceAttribute);
        }        

        #endregion

        #region Getters
        public virtual string Name
        {
            get;
            set;
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
        public virtual DateTime? DateofRetirement
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


        public virtual DateTime? DateofDeceased
        {
            get;
            set;
        }

        public virtual DateTime? DateofBirth
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual IList<Boat> Boat
        {
            get;
            set;
        }

        [JsonIgnore]
        public virtual IList<string> Marinaorgmooredtype
        {
            get;
            set;
        }
        [JsonIgnore]
        public virtual OrganisationType OrganisationType
        {
            get;
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
        [JsonIgnore]
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
        public virtual string RegisteredStatus { get; set; }
        public virtual bool ConfirmAAA { get; set; }
        public virtual string Duration { get; set; }
        [JsonIgnore]
        public virtual IList<InsuranceAttribute> InsuranceAttributes { get; set; }
        public virtual bool IsPrincipalAdvisor { get; set; }
        public virtual string OfcPhoneno { get; set; }
        public virtual string MyCRMId { get; set; }
        public virtual string TradingName { get; set; }
        public virtual string PIRetroactivedate { get; set; }
        public virtual string DORetroactivedate { get; set; }
       
        #endregion

        #region Opperations

        public virtual void ChangeOrganisationName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
        #endregion

        [JsonIgnore]
        public virtual IList<OrganisationalUnit> OrganisationalUnits { get; set; }

        public static Organisation CreateDefaultOrganisation(User creatingUser, User owner, OrganisationType organisationType)
        {
            return new Organisation(creatingUser, Guid.NewGuid(), "Default user organisation for " + owner.FullName, organisationType)
            {
                Domain = "#"
            };
        }

    }
}
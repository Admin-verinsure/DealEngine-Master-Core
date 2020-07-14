

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace DealEngine.Domain.Entities
{
    [JsonObject]
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

        public Organisation(User creator, Guid id, string organisationName, OrganisationType organisationType, List<OrganisationalUnit> organisationalUnits, InsuranceAttribute insuranceAttribute, string email)
            :this(creator, organisationName, organisationType)
        {
            Name = organisationName;
            Id = id;
            Email = email;
            OrganisationalUnits = organisationalUnits;
            InsuranceAttributes.Add(insuranceAttribute);
        }        

        #endregion

        #region Getters
        public virtual string Name
        {
            get;
            set;
        }

        public virtual DateTime? DateofRetirement
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
            get;
            set;
        }
        public virtual bool Removed
        {
            get;
            set;
        }

       // public virtual string Othercompanyname { get; set; }
        public virtual string Description { get; set; }
        public virtual string Phone { get; set; }
        public virtual string Domain { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsBroker { get; set; }
        public virtual bool IsInsurer { get; set; }
        public virtual bool IsReinsurer { get; set; }
        public virtual bool IsTC { get; set; }
        public virtual bool IsApproved { get; set; }
        public virtual IList<InsuranceAttribute> InsuranceAttributes { get; set; }
       
        #endregion

        #region Opperations

        public virtual void ChangeOrganisationName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }
        #endregion

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
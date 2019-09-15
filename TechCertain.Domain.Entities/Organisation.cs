

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TechCertain.Domain.Entities.Abstracts;

namespace TechCertain.Domain.Entities
{
    public class Organisation : EntityBase, IAggregateRoot
    {
        string _name;
        string _phone;
        string _email;
        string _SkipperExp;
        OrganisationType _organisationType;
        //IList<Product_Old> products = new List<Product_Old>();

        #region Constructors
        protected Organisation() : base(null) { }

        protected Organisation(User createdBy)
            : base(createdBy)
        {
            OrganisationalUnits = new List<OrganisationalUnit>();
            Programmes = new List<Programme>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            marinaorgmooredtype = new List<string>();
        }

        public Organisation(User createdBy, Guid id)
            : base(createdBy)
        {
            Id = id;
            OrganisationalUnits = new List<OrganisationalUnit>();
            Programmes = new List<Programme>();
            InsuranceAttributes = new List<InsuranceAttribute>();
            marinaorgmooredtype = new List<string>();
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
            marinaorgmooredtype = new List<string>();
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

        public Organisation(User createdBy, string organisationName, OrganisationType organisationType, string Email, string Phone)
           : this(createdBy, organisationName)
        {
            if (organisationType == null)
                throw new ArgumentNullException(nameof(organisationType), "Not allowed to create an organisation without specifying a type.");

            _name = organisationName;
            _email = Email;
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

        //     public Organisation(User createdBy, string organisationName, OrganisationType organisationType, string ownerFirstName, string ownerLastName, string ownerEmail)
        //: this (createdBy, organisationName, organisationType)
        //     {
        //         if (string.IsNullOrWhiteSpace(organisationName))
        //             throw new ArgumentNullException("name", "Not allowed to create an organisation with no name.");

        //         if (organisationType == null)
        //             throw new ArgumentNullException("organisationType", "Not allowed to create an organisation without specifying a type.");

        //  this.name = organisationName;
        //         this.organisationType = organisationType;


        //         AddNewAccount(ownerFirstName, ownerLastName, ownerEmail);
        //     }
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

        public virtual IList<Boat> Boat
        {
            get;
            set;
        }

        public virtual IList<string> marinaorgmooredtype
        {
            get;
            set;
        }

        //public virtual IEnumerable<Product_Old> Products
        //{
        //    get { return products; }
        //}

        public virtual OrganisationType OrganisationType
        {
            //  set {}

            get { return _organisationType; }
        }

        public virtual IList<Territory> territory
        {
            get;
            set;
        }

        public virtual Location Location { get; set; }

        public virtual string Description { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Domain { get; set; }

        //public virtual string Email { get; set; }

        public virtual bool IsBroker { get; set; }

        public virtual bool IsInsurer { get; set; }

        public virtual bool IsReinsurer { get; set; }

        public virtual bool IsTC { get; set; }
        public virtual bool IsApproved { get; set; }

        public virtual IList<Programme> Programmes
        {
            get;
            set;
        }

        public virtual IList<InsuranceAttribute> InsuranceAttributes { get; set; }

        #endregion

        #region Opperations
        //public virtual void AddProduct(Product_Old product)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException("Product can not be null");

        //    products.Add(product);
        //}

        //public virtual Product_Old AddNewProduct(string productName)
        //{
        //    Product_Old product = new Product_Old(CreatedBy, productName);

        //    products.Add(product);

        //    return product;
        //}

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


        //public virtual IEnumerable<User> Users { get; set; }

        public virtual IList<OrganisationalUnit> OrganisationalUnits { get; set; }

        //public virtual IEnumerable<Department> Departments { get; set; }

        //public virtual IEnumerable <BusinessType> BusinessTypes { get; set; }

        public static Organisation CreateDefaultOrganisation(User creatingUser, User owner, OrganisationType organisationType)
        {
            //return new Organisation (creatingUser, Guid.NewGuid (), "Default user organisation for " + owner.FullName, new OrganisationType (creatingUser, "personal")) {
            return new Organisation(creatingUser, Guid.NewGuid(), "Default user organisation for " + owner.FullName, organisationType)
            {
                Domain = "#"
            };
        }

        public static Organisation CreateOrganisation(User creatingUser, string organisationName, OrganisationType organisationType, string Email, string Phone)
        {
            //return new Organisation (creatingUser, Guid.NewGuid (), "Default user organisation for " + owner.FullName, new OrganisationType (creatingUser, "personal")) {
            return new Organisation(creatingUser, organisationName, organisationType, Email, Phone)
            {
                Domain = "#"
            };
        }
    }
}
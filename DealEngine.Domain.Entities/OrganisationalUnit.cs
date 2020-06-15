using System;
using System.Collections;
using System.Collections.Generic;
using DealEngine.Domain.Entities.Abstracts;
using Microsoft.AspNetCore.Http;

namespace DealEngine.Domain.Entities
{
	public class OrganisationalUnit : EntityBase, IAggregateRoot
    {
        protected OrganisationalUnit () : base (null) {}

        public OrganisationalUnit(User createdBy, string name) 
			: base (createdBy)
		{
            Name = name;
            Locations = new List<Location>();
            BranchCodes = new List<BranchCode>();
            Marinaorgmooredtype = new List<string>();
        }

        public OrganisationalUnit(User createdBy, string name, IFormCollection collection)
            : base(createdBy)
        {
            Locations = new List<Location>();
            BranchCodes = new List<BranchCode>();
            if(collection != null)
            {
                PopulateEntity(collection);
            }
            Name = name;
        }

        public virtual string Name { get; set; }

		public virtual IEnumerable<BranchCode> BranchCodes { get; set; }

        public virtual IList<Location> Locations { get; set; }
        public virtual IList<string> Marinaorgmooredtype { get; set; }

        public virtual string EserviceProducerCode { get; set; }

        public virtual string EbixDepartmentCode { get; set; }

        public virtual string HPFBranchCode { get; set; }
        public virtual bool IsCurrentMembership { get; set; }
        // public virtual string Othercompanyname { get; set; }
        public virtual string DateQualified { get; set; }
        public virtual string DesignLicensed { get; set; }
        public virtual string SiteLicensed { get; set; }
        public virtual bool IsRegisteredLicensed { get; set; }
        public virtual bool IsPrincipalAdvisor { get; set; }
        public virtual string OfcPhoneno { get; set; }
        public virtual string MyCRMId { get; set; }
        public virtual string TradingName { get; set; }
        public virtual string PIRetroactivedate { get; set; }
        public virtual string DORetroactivedate { get; set; }
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
    }
}
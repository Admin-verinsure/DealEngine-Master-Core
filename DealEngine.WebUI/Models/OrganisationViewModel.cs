using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace DealEngine.WebUI.Models
{
    public class OrganisationViewModel : BaseViewModel
    {
        public OrganisationViewModel() { }
        public OrganisationViewModel(ClientInformationSheet ClientInformationSheet, Organisation organisation, User OrgUser)
        {
            User = new User(null, Guid.NewGuid());
            if(organisation != null)
            {
                Organisation = organisation;
            }
            if (ClientInformationSheet != null)
            {
                Programme = ClientInformationSheet.Programme.BaseProgramme;
                Types = GetTypes();
                OrganisationTypes = GetOrganisationTypes();
                HasRetiredorDecievedOptions = GetHasRetiredorDecievedOptions();
                HasRegisteredOptions = GetHasRegisteredOptions();
                Organisations = ClientInformationSheet.Organisation;
            }
            if(OrgUser != null)
            {
                User = OrgUser;
            }
        }

        private IList<SelectListItem> GetHasRetiredorDecievedOptions()
        {
            var _Types = new List<SelectListItem>()
            {
                new SelectListItem
                    {
                        Text = "Yes",
                        Value = "True"
                    },
                    new SelectListItem
                    {
                        Text = "No",
                        Value = "False"
                    }
            };
            return _Types;
        }

        private IList<SelectListItem> GetOrganisationTypes()
        {
            var _Types = new List<SelectListItem>();
            if (Programme.Name == "NZFSG Programme")
            {
                _Types = new List<SelectListItem>() {
                    new SelectListItem
                    {
                        Text = "Private",
                        Value = "Person - Individual"
                    },
                    new SelectListItem
                    {
                        Text = "Corporation – Limited liability",
                        Value = "Company"
                    },
                    new SelectListItem
                    {
                        Text = "Trust",
                        Value = "Trust"
                    },
                    new SelectListItem
                    {
                        Text = "Partnership",
                        Value = "Partnership"
                    }
                };
            }
            return _Types;
        }
        private IList<SelectListItem> GetHasRegisteredOptions()
        {
            var _Types = new List<SelectListItem>();
            _Types = new List<SelectListItem>() {
                    new SelectListItem
                    {
                        Text = "AFA",
                        Value = "AFA"
                    },
                    new SelectListItem
                    {
                        Text = "RFA",
                        Value = "RFA"
                    },
                    new SelectListItem
                    {
                        Text = "N/A",
                        Value = "N/A"
                    }
                };
            return _Types;
        }
        private IList<SelectListItem> GetTypes()
        {
            var _Types = new List<SelectListItem>();
            if (Programme.Name == "NZFSG Programme")
            {
                _Types = new List<SelectListItem>() {
                    new SelectListItem
                    {
                        Text = "Advisor",
                        Value = "Advisor"
                    },
                    new SelectListItem
                    {
                        Text = "Nominated Representative",
                        Value = "NominatedRepresentative"
                    },
                    new SelectListItem
                    {
                        Text = "Other Consulting Business",
                        Value = "OtherConsultingBusiness"
                    }
                };

            }
            return _Types;

        }
        public DealEngine.Domain.Entities.Programme Programme { get; set; }

        public Guid ID { get; set; }
        public Guid ProgrammeId { get; set; }
        public Organisation Organisation { get; set; }
        public User User { get; set; }
        [Display(Name ="Type")]
        public IList<SelectListItem> Types { get; set; }
        public IList<SelectListItem> OrganisationTypes { get; set; }
        public IList<SelectListItem> HasRetiredorDecievedOptions { get; set; }
        public IList<SelectListItem> HasRegisteredOptions { get; set; }
        public IList<Organisation> Organisations { get; set; }

        #region OLD!
        // Organisation Details --- 
        public string OrganisationName { get; set; }
        public string OrganisationEmail { get; set; }
        public string OrganisationPhone { get; set; }

        public string OrganisationTypeName { get; set; }
        public string InsuranceAttribute { get; set; }

        public Guid AnswerSheetId { get; set; }
        public IList<SelectListItem> OrgMooredType { get; set; }

        public string OperatorYearsOfExp { get; set; }
        public IList<User> Users { get; set; }

        public string IsAdmin { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public bool IsPrimary { get; set; }
        public string Qualifications { get; set; }
        public bool IsNZIAmember { get; set; }
        public string IsIPENZmember { get; set; }
        public string NZIAmembership { get; set; }
        public string YearofPractice { get; set; }
        public string prevPractice { get; set; }
        public string Type { get; set; }
        public bool IsADNZmember { get; set; }
        public string DateofRetirement { get; set; }
        public string CPEngQualified { get; set; }
        public string DateofDeceased { get; set; }
        public string DateofBirth { get; set; }
        public bool IsLPBCategory3 { get; set; }
        public bool IsRetiredorDecieved { get; set; }
        public bool IsOtherdirectorship { get; set; }
        public string Othercompanyname { get; set; }
        public string Activities { get; set; }
        public string ProfAffiliation { get; set; }
        public string JobTitle { get; set; }
        public string InsuredEntityRelation { get; set; }
        public bool IsContractorInsured { get; set; }
        public bool IsCurrentMembership { get; set; }
        public bool IsInsuredRequired { get; set; }
        public string PMICert { get; set; }
        public string PartyName { get; set; }
        public string CertType { get; set; }
        public bool MajorShareHolder { get; set; }
        public bool isaffiliation { get; set; }
        public string affiliationdetails { get; set; }
        public string CurrentMembershipNo { get; set; }
        public string DateQualified { get; set; }
        public string DesignLicensed { get; set; }
        public string SiteLicensed { get; set; }
        public bool IsRegisteredLicensed { get; set; }
        public bool ConfirmAAA { get; set; }
        public string RegisteredStatus { get; set; }
        public string Duration { get; set; }
        public bool IsPrincipalAdvisor { get; set; }
        public string OfcPhoneno { get; set; }
        public string MyCRMId { get; set; }
        public string TradingName { get; set; }
        #endregion

        public static OrganisationViewModel FromEntity(OrganisationViewModel organisationViewModel)
        {
            OrganisationViewModel model = new OrganisationViewModel
            {
                OrganisationTypeName = organisationViewModel.OrganisationTypeName,
                OrganisationName = organisationViewModel.OrganisationName,
                OrganisationEmail = organisationViewModel.OrganisationEmail,
                OrganisationPhone = organisationViewModel.OrganisationPhone,
            };

            return model;
        }

    }


}



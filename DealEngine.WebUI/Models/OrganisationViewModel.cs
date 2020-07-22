using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Linq;

namespace DealEngine.WebUI.Models
{
    public class OrganisationViewModel : BaseViewModel
    {
        public OrganisationViewModel() { }
        public OrganisationViewModel(ClientInformationSheet ClientInformationSheet, Organisation organisation, User OrgUser)
        {
            User = new User(null, Guid.NewGuid());
            Organisations = new List<Organisation>();
            
            OrganisationTypes = GetOrganisationTypes();
            if (organisation != null)
            {
                Organisation = organisation;
            }
            if (ClientInformationSheet != null)
            {
                Programme = ClientInformationSheet.Programme.BaseProgramme;
                if(Programme.Name == "NZFSG Programme" || Programme.Name == "TripleA Programme")
                {
                    AdvisorUnit = new AdvisorUnit(null, null, null, null);//organisation.FirstOrDefault(o=>o.OrganisationalUnits.Any(o=>o.Type == "Advisor"));
                    InsuranceAttributes = GetAdvisorTypes();
                    HasRetiredorDeceasedOptions = GetStandardSelectOptions();
                    HasRegisteredOptions = GetHasRegisteredOptions();
                    OrganisationTypes = GetOrganisationTypes();
                    HasPrincipalAdvisor = GetStandardSelectOptions();
                    HasIsTripleAApprovalOptions = GetStandardSelectOptions();
                }
                if (Programme.Name == "DANZ Programme" || Programme.Name == "PMINZ Programme")
                {
                    InsuranceAttributes = GetPersonnelTypes();
                    PersonnelUnit = new PersonnelUnit(null, null, null, null); //(PersonnelUnit)organisation.OrganisationalUnits.FirstOrDefault(o => o.Type == "Personnel");
                    InsuredEntityRelationOptions = GetInsuredEntityRelationOptions();
                    HasRegisteredLicensedOptions = GetStandardSelectOptions();
                    HasDesignLicencedOptions = GetLicencedOptions();
                    HasSiteLicensedOptions = GetLicencedOptions();
                    HasCurrentMembershipOptions = GetStandardSelectOptions();
                    InsuredEntityRelationOptions = GetInsuredEntityRelationOptions();
                    HasContractorInsuredOptions = GetStandardSelectOptions();
                    HasInsuredRequiredOptions = GetStandardSelectOptions();
                    HasCurrentMembershipOptions = GetStandardSelectOptions();
                    CertTypes = GetCertTypes();
                    HasMajorShareHolder = GetStandardSelectOptions();
                }
                if (Programme.Name == "CEAS Programme" || Programme.Name == "NZACS Programme")
                {
                    InsuranceAttributes = GetPrincipalTypes();
                    PrincipalUnit = new PrincipalUnit(null, null, null, null); //(PrincipalUnit)organisation.OrganisationalUnits.FirstOrDefault(o => o.Type == "Principal");
                    HasRetiredorDeceasedOptions = GetStandardSelectOptions();
                    HasIsIPENZmemberOptions = GetStandardSelectOptions();
                    HasCPEngQualifiedOptions = GetStandardSelectOptions();
                    HasIsNZIAmemberOptions = GetStandardSelectOptions();
                    HasIsADNZmemberOptions = GetStandardSelectOptions();
                    HasIsOtherdirectorshipOptions = GetStandardSelectOptions();
                }

                Organisations.Add(ClientInformationSheet.Owner);
                if (ClientInformationSheet.Organisation.Any())
                {
                    foreach(var sheetOrg in ClientInformationSheet.Organisation)
                    {
                        Organisations.Add(sheetOrg);
                    }                    
                }
                
            }
            if(OrgUser != null)
            {
                User = OrgUser;
            }
        }

        private IList<SelectListItem> GetPrincipalTypes()
        {
            var _Types = new List<SelectListItem>();
            _Types = new List<SelectListItem>() {
                    new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    },
                    new SelectListItem
                    {
                        Text = "Principal",
                        Value = "Principal"
                    },
                    new SelectListItem
                    {
                        Text = "Subsidiary",
                        Value = "Subsidiary"
                    },
                    new SelectListItem
                    {
                        Text = "Previous Consulting Business",
                        Value = "Previous Consulting Business"
                    },
                    new SelectListItem
                    {
                        Text = "Mergers",
                        Value = "Mergers"
                    },
                    new SelectListItem
                    {
                        Text = "Joint Venture",
                        Value = "JointVenture"
                    },
                    new SelectListItem
                    {
                        Text = "Previous Consulting Business",
                        Value = "Previous Consulting Business"
                    }
                };
            return _Types;
        }
        private IList<SelectListItem> GetInsuredEntityRelationOptions()
        {
            var _Types = new List<SelectListItem>()
            {
                new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Director",
                        Value = "Director"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Employee",
                        Value = "Employee"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Contractor",
                        Value = "Contractor"
                    }
            };
            return _Types;
        }
        private List<SelectListItem> GetCertTypes()
        {
            var _Types = new List<SelectListItem>()
            {
                new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Ordinary/Non Member",
                        Value = "Ordinary"
                    }
                ,
                new SelectListItem
                    {
                        Text = "PMP",
                        Value = "PMP"
                    }
                ,
                new SelectListItem
                    {
                        Text = "CAPM",
                        Value = "CAPM"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Project Director",
                        Value = "ProjectDirector"
                    }
            };
            return _Types;
        }
        private IList<SelectListItem> GetPersonnelTypes()
        {
            var _Types = new List<SelectListItem>();
            _Types = new List<SelectListItem>() {
                    new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    },
                    new SelectListItem
                    {
                        Text = "Personnel",
                        Value = "Personnel"
                    },
                    new SelectListItem
                    {
                        Text = "Subsidiary",
                        Value = "Subsidiary"
                    },
                    new SelectListItem
                    {
                        Text = "Previous Consulting Business",
                        Value = "Previous Consulting Business"
                    }
                    ,
                    new SelectListItem
                    {
                        Text = "Mergers",
                        Value = "Mergers"
                    }
                    ,
                    new SelectListItem
                    {
                        Text = "Joint Venture",
                        Value = "Joint Venture"
                    }
                    ,
                    new SelectListItem
                    {
                        Text = "Major Share Holder (Not being a PM)",
                        Value = "Major Share Holder"
                    }
                };
            return _Types;
        }
        //private IList<SelectListItem> GetDANZTypes()
        //{
        //    var _Types = new List<SelectListItem>();
        //    _Types = new List<SelectListItem>() {
        //            new SelectListItem
        //            {
        //                Text = "-- Select --",
        //                Value = "0"
        //            },
        //            new SelectListItem
        //            {
        //                Text = "Personnel",
        //                Value = "Personnel"
        //            },
        //            new SelectListItem
        //            {
        //                Text = "Subsidiary",
        //                Value = "Subsidiary"
        //            },
        //            new SelectListItem
        //            {
        //                Text = "Previous Consulting Business",
        //                Value = "Previous Consulting Business"
        //            }
        //            ,
        //            new SelectListItem
        //            {
        //                Text = "Mergers",
        //                Value = "Mergers"
        //            }
        //            ,
        //            new SelectListItem
        //            {
        //                Text = "Joint Venture",
        //                Value = "Joint Venture"
        //            }
        //        };
        //    return _Types;
        //}
        private IList<SelectListItem> GetLicencedOptions()
        {
            var _Types = new List<SelectListItem>()
            {
                new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    }
                ,
                new SelectListItem
                    {
                        Text = "None",
                        Value = "None"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Category 1",
                        Value = "Category 1"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Category 2",
                        Value = "Category 2"
                    }
                ,
                new SelectListItem
                    {
                        Text = "Category 3",
                        Value = "Category 3"
                    }
            };
            return _Types;
        }
        private IList<SelectListItem> GetStandardSelectOptions()
        {
            var _Types = new List<SelectListItem>()
            {
                new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    },
                new SelectListItem
                    {
                        Text = "No",
                        Value = "false"
                    },
                new SelectListItem
                    {
                        Text = "Yes",
                        Value = "true"
                    }
            };
            return _Types;
        }
        private IList<SelectListItem> GetOrganisationTypes()
        {
            var _Types = new List<SelectListItem>();
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
            return _Types;
        }
        private IList<SelectListItem> GetHasRegisteredOptions()
        {
            var _Types = new List<SelectListItem>();
            _Types = new List<SelectListItem>() {                            
                new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    },
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
        private IList<SelectListItem> GetAdvisorTypes()
        {
            var _Types = new List<SelectListItem>();
            _Types = new List<SelectListItem>() {
                new SelectListItem
                    {
                        Text = "-- Select --",
                        Value = "0"
                    },
                new SelectListItem
                    {
                        Text = "Advisor",
                        Value = "Advisor"
                    },
                new SelectListItem
                    {
                        Text = "Administration",
                        Value = "Administration"
                    },
                new SelectListItem
                {
                    Text = "Nominated Representative",
                    Value = "Nominated Representative"
                },
                new SelectListItem
                {
                    Text = "Other Consulting Business",
                    Value = "Other Consulting Business"
                }
            };
            return _Types;

        }
        [JsonIgnore]
        public Domain.Entities.Programme Programme { get; set; }
        public Guid ID { get; set; }
        public Guid ProgrammeId { get; set; }
        public Organisation Organisation { get; set; }
        public User User { get; set; }
        [Display(Name ="Type")]
        [JsonIgnore]
        public IList<SelectListItem> InsuranceAttributes { get; set; }
        [Display(Name = "Organisation Type")]
        [JsonIgnore]
        public IList<SelectListItem> OrganisationTypes { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasRetiredorDeceasedOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasRegisteredOptions { get; set; }
        [JsonIgnore]
        public IList<Organisation> Organisations { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasPrincipalAdvisor { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasRegisteredLicensedOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasDesignLicencedOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasSiteLicensedOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasCurrentMembershipOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasContractorInsuredOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasInsuredRequiredOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> InsuredEntityRelationOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> CertTypes { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasIsTripleAApprovalOptions { get; set; }        
        [JsonIgnore]
        public IList<SelectListItem> HasMajorShareHolder { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasIsIPENZmemberOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasCPEngQualifiedOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasIsNZIAmemberOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasIsADNZmemberOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasIsOtherdirectorshipOptions { get; set; }


        public AdvisorUnit AdvisorUnit { get; set; }
        public PersonnelUnit PersonnelUnit { get; set; }
        public PrincipalUnit PrincipalUnit { get; set; }


        #region OLD!
        // Organisation Details --- 
        public string OrganisationType { get; set; }
        public string InsuranceAttribute { get; set; }
        public Guid AnswerSheetId { get; set; }
        
        #endregion
    }


}



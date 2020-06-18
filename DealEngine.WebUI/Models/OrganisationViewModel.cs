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
            Types = GetTypes();
            HasRetiredorDecievedOptions = GetHasRetiredorDecievedOptions();
            HasRegisteredOptions = GetHasRegisteredOptions();
            OrganisationTypes = GetOrganisationTypes();
            HasPrincipalAdvisor = GetHasPrincipalAdvisor();
            if (organisation != null)
            {
                Organisation = organisation;
            }
            if (ClientInformationSheet != null)
            {
                Programme = ClientInformationSheet.Programme.BaseProgramme;
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

        private IList<SelectListItem> GetHasPrincipalAdvisor()
        {
            var _Types = new List<SelectListItem>()
            {
                new SelectListItem
                    {
                        Text = "No",
                        Value = "False"
                    },
                new SelectListItem
                    {
                        Text = "Yes",
                        Value = "True"
                    }
            };
            return _Types;
        }

        private IList<SelectListItem> GetHasRetiredorDecievedOptions()
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
            //if (Programme.Name == "NZFSG Programme")
            //{
            //    _Types = new List<SelectListItem>() {
            //        new SelectListItem
            //        {
            //            Text = "Private",
            //            Value = "Person - Individual"
            //        },
            //        new SelectListItem
            //        {
            //            Text = "Corporation – Limited liability",
            //            Value = "Company"
            //        },
            //        new SelectListItem
            //        {
            //            Text = "Trust",
            //            Value = "Trust"
            //        },
            //        new SelectListItem
            //        {
            //            Text = "Partnership",
            //            Value = "Partnership"
            //        }
            //    };
            //}
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
                        Text = "Nominated Representative",
                        Value = "NominatedRepresentative"
                    },
                    new SelectListItem
                    {
                        Text = "Other Consulting Business",
                        Value = "OtherConsultingBusiness"
                    }
                };
            //if (Programme.Name == "NZFSG Programme")
            //{
                

            //}
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
        public IList<SelectListItem> Types { get; set; }
        [Display(Name = "Organisation Type")]
        [JsonIgnore]
        public IList<SelectListItem> OrganisationTypes { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasRetiredorDecievedOptions { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasRegisteredOptions { get; set; }
        [JsonIgnore]
        public IList<Organisation> Organisations { get; set; }
        [JsonIgnore]
        public IList<SelectListItem> HasPrincipalAdvisor { get; set; }

        #region OLD!
        // Organisation Details --- 
        public string OrganisationType { get; set; }
        public string InsuranceAttribute { get; set; }
        public Guid AnswerSheetId { get; set; }
        
        #endregion
    }


}



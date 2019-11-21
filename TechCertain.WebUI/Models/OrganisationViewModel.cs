using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models
{
    public class OrganisationViewModel : BaseViewModel
    {
        public Guid ID { get; set; }
        public Guid ProgrammeId { get; set; }

        // Organisation Details
        public string OrganisationName { get; set; }
        public string OrganisationEmail { get; set; }
        public string OrganisationPhone { get; set; }

        public string OrganisationTypeName { get; set; }
        public string InsuranceAttribute { get; set; }

        public Guid AnswerSheetId { get; set; }
        public Guid PartyUseId { get; set; }
        public OrganisationType OrganisationType { get; set; }
        public IList<SelectListItem> OrgMooredType { get; set; }

        public string OperatorYearsOfExp { get; set; }

        public User user { get; set; }

        public IList<User> Users { get; set; }
        public IEnumerable<string> OrganisationTypes { get; set; }
        public string IsAdmin { get; set; }

        // Organisation Owner Details
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        public bool IsPrimary { get; set; }

        public string YearsOfExp { get; set; }

        public virtual IList<OrganisationalUnit> OrganisationalUnits { get; set; }



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
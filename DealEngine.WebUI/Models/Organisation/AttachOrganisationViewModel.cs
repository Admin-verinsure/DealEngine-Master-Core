
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DealEngine.WebUI.Models.Organisation
{
    public class AttachOrganisationViewModel : BaseViewModel
    {
        public AttachOrganisationViewModel(List<ClientInformationSheet> sheets, List<Domain.Entities.Organisation> removedOrganisations)
        {
            PopulateOwnerList(sheets);
            PopulateOrganisationRemovedList(removedOrganisations);
        }

        private void PopulateOrganisationRemovedList(List<Domain.Entities.Organisation> removedOrganisations)
        {
            RemovedOrganisations = new List<SelectListItem>();
            foreach (var org in removedOrganisations)
            {
                RemovedOrganisations.Add(new SelectListItem()
                {
                    Text = org.Name,
                    Value = org.Id.ToString()
                });
            }
        }

        private void PopulateOwnerList(List<ClientInformationSheet> sheets)
        {
            Owners = new List<SelectListItem>();
            foreach(var sheet in sheets)
            {
                Owners.Add(new SelectListItem()
                {
                    Text = sheet.Owner.Name,
                    Value= sheet.Owner.Id.ToString()
                });
            }            
        }

        [Display(Name = "List of Owners (Insured's)")]
        public IList<SelectListItem> Owners { get; set; }
        [Display(Name = "List removed Parties")]
        public IList<SelectListItem> RemovedOrganisations { get; set; }
    }
}



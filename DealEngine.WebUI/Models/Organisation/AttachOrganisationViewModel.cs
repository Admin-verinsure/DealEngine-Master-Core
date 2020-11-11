﻿
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DealEngine.WebUI.Models.Organisation
{
    public class AttachOrganisationViewModel : BaseViewModel
    {
        public AttachOrganisationViewModel(IList<ClientProgramme> clientProgrammes, Domain.Entities.Organisation removedOrganisation)
        {
            PopulateOwnerList(clientProgrammes);
            RemovedOrganisation = removedOrganisation;
        }
        private void PopulateOwnerList(IList<ClientProgramme> clientProgrammes)
        {
            Owners = new List<SelectListItem>();
            // Add Change Programmes
            foreach (var clientProgramme in clientProgrammes.Where(p => p.DateDeleted == null).Where(p => p.InformationSheet.IsChange == true).ToList())
            {
                Owners.Add(new SelectListItem()
                {
                    Text = clientProgramme.InformationSheet.Owner.Name + " Reference: " + clientProgramme.InformationSheet.ReferenceId
                    + " Status: " + clientProgramme.InformationSheet.Status,
                    Value = clientProgramme.Id.ToString()
                });
            }
            // Add Original Programmes
            foreach (var clientProgramme in clientProgrammes.Where(p => p.DateDeleted == null).Where(p => p.InformationSheet.IsChange == false).Where(p => p.InformationSheet.NextInformationSheet == null).ToList())
            {
                Owners.Add(new SelectListItem()
                {
                    Text = clientProgramme.InformationSheet.Owner.Name + " Reference: "+ clientProgramme.InformationSheet.ReferenceId
                    + " Status: "+ clientProgramme.InformationSheet.Status,
                    Value = clientProgramme.Id.ToString()
                });
            }
            Owners = Owners.OrderBy(f => f.Text).ToList();
        }
        [Display(Name = "List of Owners (Insured's)")]
        public IList<SelectListItem> Owners { get; set; }
        [Display(Name = "Organisation Requesting to join")]
        public Domain.Entities.Organisation RemovedOrganisation { get; set; }
    }
}



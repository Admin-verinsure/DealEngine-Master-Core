﻿
using DealEngine.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
            foreach(var clientProgramme in clientProgrammes)
            {
                Owners.Add(new SelectListItem()
                {
                    Text = clientProgramme.InformationSheet.Owner.Name,
                    Value= clientProgramme.InformationSheet.Owner.Id.ToString()
                });
            }            
        }

        [Display(Name = "List of Owners (Insured's)")]
        public IList<SelectListItem> Owners { get; set; }
        [Display(Name = "Organisation Requesting to join")]
        public Domain.Entities.Organisation RemovedOrganisation { get; set; }
    }
}


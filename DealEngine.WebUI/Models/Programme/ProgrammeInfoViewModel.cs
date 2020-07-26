using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;
using DealEngine.WebUI.Models.ProductModels;
using System.ComponentModel.DataAnnotations;

namespace DealEngine.WebUI.Models.Programme
{


	public class ProgrammeInfoViewModel : BaseViewModel
	{
        public ProgrammeInfoViewModel() 
        {
            Brokers = new List<SelectListItem>();
        }
        public ProgrammeInfoViewModel(List<User> brokers, Domain.Entities.Programme programme, ClientProgramme clientProgramme)
        {
            if(brokers != null)
            {
                Brokers = GetBrokerSelectList(brokers);
            }            
            if(programme != null)
            {
                Id = programme.Id;
                Name = programme.Name;
                this.Programme = programme;
            }
            if(clientProgramme != null)
            {
                ClientProgramme = clientProgramme;
            }
        }

        private IList<SelectListItem> GetBrokerSelectList(List<User> brokers)
        {
            Brokers = new List<SelectListItem>();
            foreach (var broker in brokers)
            {
                Brokers.Add(
                    new SelectListItem
                    {
                        Text = broker.FirstName + " " + broker.Email,
                        Value = broker.Id.ToString(),
                        Selected = false
                    });
            }
            return Brokers;
        }

        public Guid Id { get; set; }
        public bool EGlobalIsActiveOrNot { get; set; }
        public ClientProgramme ClientProgramme { get; set; }
        public IList<ProductInfoViewModel> Product { get; set; }
        public IList<Organisation> Owner { get; set; }
        public IList<EGlobalSubmission> EGlobalSubmissions { get; set; }
        public IList<EGlobalResponse> EGlobalResponses { get; set; }
        public User BrokerContactUser { get; set; }
        public string Name { get; set; }
        public Domain.Entities.Programme Programme { get; }
        public string OwnerCompany { get; set; }
        public string DateCreated { get; set; }
        public IList<SelectListItem> Brokers { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public InformationBuilderViewModel InformationBuilderViewModel { get; set; }
        public Guid AgreementId { get; set; }
        public Guid ProgId { get; set; }
        public IList<Rule> Rules { get; set; }
        public Guid ProductId { get; set; }

    }
}


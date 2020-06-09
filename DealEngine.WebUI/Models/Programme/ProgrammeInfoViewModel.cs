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
        public ProgrammeInfoViewModel() { }
        public ProgrammeInfoViewModel(List<User> brokers)
        {
            Brokers = GetBrokerSelectList(brokers);
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
        public Guid OwnerId { get; set; }
        public Guid ProductId { get; set; }
        public Guid selectedparty { get; set; }
        public IList<Organisation> Parties { get; set; }
        public IList<ClientProgramme> clientProgrammes { get; set; }
        public IList<ProductInfoViewModel> Product { get; set; }
        public IList<Rule> Rules { get; set; }
        public IList<Organisation> Owner { get; set; }
        public IList<SelectListItem> OrgUser { get; set; }
        public IList<EGlobalSubmission> EGlobalSubmissions { get; set; }
        public IList<EGlobalResponse> EGlobalResponses { get; set; }
        public User BrokerContactUser { get; set; }
        public ClientProgramme clientprogramme { get; set; }
        public string programmeName { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string OwnerCompany { get; set; }
        public string DateCreated { get; set; }
        public string LocalDateSubmitted { get; set; }
        public string PolicyNumberPrefixString { get; set; }
        public string EGlobalBranchCode { get; set; }
        public string EGlobalClientNumber { get; set; }
        public string EGlobalClientStatus { get; set; }
        public string EGlobalCustomDescription { get; set; }
        public string ProgrammeClaim { get; set; }
        public bool HasEGlobalCustomDescription { get; set; }
        public bool EGlobalIsActiveOrNot { get; set; }        
        public bool IsPublic { get; set; }
        public bool UsesEGlobal { get; set; }
        public bool StopAgreement { get; set; }
        public bool HasSubsystemEnabled { get; set; }
        public decimal TaxRate { get; set; }
        public DateTime StopAgreementDateTime { get; set; }
        public IList<SelectListItem> Brokers { get; set; }
        public string Declaration { get; set; }
        public string StopAgreementMessage { get; set; }
        public string NoPaymentRequiredMessage { get; set; }
        public ProductViewModel ProductViewModel { get; set; }
        public InformationBuilderViewModel InformationBuilderViewModel { get; set; }

    }
}


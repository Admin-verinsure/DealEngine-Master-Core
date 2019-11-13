using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TechCertain.Domain.Entities;
using TechCertain.WebUI.Models.Product;

namespace TechCertain.WebUI.Models.Programme
{


	public class ProgrammeInfoViewModel : BaseViewModel
	{
		public Guid Id { get; set; }        
        public string programmeName { get; set; }
        public string Name { get; set; }
        public User BrokerContactUser { get; set; }
        public Guid OwnerId { get; set; }
        public string Status { get; set; }
        public string OwnerCompany { get; set; }
        public string DateCreated { get; set; }
        public Guid ProductId { get; set; }
        public string LocalDateSubmitted { get; set; }
        public  IList<Organisation> Parties { get; set; }
        public Guid selectedparty { get; set; }
        public IList<ClientProgramme> clientProgrammes { get; set; }
        public  IList<ProductInfoViewModel> Product { get;  set; }
        public virtual IList<Rule> Rules { get;  set; }
        public List<Organisation> Owner { get; set; }
        public ClientProgramme clientprogramme { get; set; }
        public virtual string EGlobalBranchCode { get; set; }
        public virtual string EGlobalClientNumber { get; set; }
        public virtual string EGlobalClientStatus { get; set; }
        public virtual bool HasEGlobalCustomDescription { get; set; }
        public virtual string EGlobalCustomDescription { get; set; }
        public virtual decimal TaxRate { get; set; }
        public virtual Boolean IsPublic { get; set; }
        public virtual Boolean UsesEGlobal { get; set; }

        //public IList<EmailTemplates> emailTemplate { get; set; }

        //public class ProgrammeItem : BaseViewModel
        //{
        //    //public string Name { get; set; }

        //    //public string Description { get; set; }

        //    public string ProgrammeId { get; set; }

        ////    public IList<string> Languages { get; set; }

        ////    public string DisplayRole { get; set; }

        ////    public IList<DealItem> Deals { get; set; }
        //}
        public List<SelectListItem> OrgUser { get; set; }

    }
}


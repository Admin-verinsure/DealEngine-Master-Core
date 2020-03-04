using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Programme
{


	public class ClientProgrammeInfoViewModel : BaseViewModel
	{
		public Guid Id { get; set; }
        public Guid ProgramId { get; set; }
        public Guid OwnerId { get; set; }


        public string Phone { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Brokercontact { get; set; }

        public string Status { get; set; }

		public DateTime DateCreated { get; set; }

        public DateTime DateIssued { get; set; }

        public DateTime Datestarted { get; set; }

        public DateTime LastLogon { get; set; }

        public string Submitted { get; set; }




        //public IList<ClientProgramme> clientProgrammes { get; set; }

        //public List<Organisation> Owner { get; set; }
        //public ClientProgramme clientprogramme { get; set; }




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



    }
}


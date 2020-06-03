using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models
{
	public class IssueUISViewModel : BaseViewModel
	{		
		public List<ClientProgramme> ClientProgrammes { get; set; }
		public string ProgrammeId { get; set; }
		public List<User> users { get; set; }
	}
}


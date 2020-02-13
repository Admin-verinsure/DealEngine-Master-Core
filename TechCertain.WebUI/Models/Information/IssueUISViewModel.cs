using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models
{
	public class IssueUISViewModel : BaseViewModel
	{		
		public List<ClientProgramme> ClientProgrammes { get; set; }
		public string ProgrammeId { get; set; }
	}
}


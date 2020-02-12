using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models
{
	public class IssueUISViewModel : BaseViewModel
	{
		public List<ClientProgramme> ClientProgrammes { get; internal set; }
	}
}


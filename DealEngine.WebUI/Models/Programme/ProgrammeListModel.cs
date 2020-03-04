
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.Programme
{

	public class ProgrammeListModel : BaseViewModel
	{
		public IList<Domain.Entities.Programme> Programmes { get; set; }
    }
}


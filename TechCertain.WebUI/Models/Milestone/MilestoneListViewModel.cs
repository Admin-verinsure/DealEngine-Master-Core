using System.Collections.Generic;

namespace TechCertain.WebUI.Models.Milestone
{
    public class MilestoneListViewModel : BaseViewModel
    {
        public IList<MilestoneConfigurationViewModel> MilestoneVM { get; set; }
        public string ProgrammeId { get; set; }
        public string MilestoneActivity { get; set; }

    }

    public class MilestoneConfigurationViewModel
    {
        public string Process { get; set; }
        public IList<TechCertain.Domain.Entities.Programme> Programmes { get; set; }
        public bool IsActive { get; set; }
    }

}

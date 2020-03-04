using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Milestone
{
    public class MilestoneListViewModel : BaseViewModel
    {
        public IList<MilestoneConfigurationViewModel> MilestoneVM { get; set; }
        public string ProgrammeId { get; set; }
        public string MilestoneActivity { get; set; }

    }

    public class MilestoneConfigurationViewModel
    {
        public ProgrammeProcess ProgrammeProcess { get; set; }
        public IList<Domain.Entities.Programme> Programmes { get; set; }
        public bool IsActive { get; set; }
    }

}

using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Milestone
{
    //public class MilestoneViewModel : BaseViewModel
    //{
    //    public IDictionary<string, MilestoneBuilderViewModel> MilestoneBuilderViewModel { get; set; }

    //}

    public class MilestoneBuilderViewModel : BaseViewModel
    {
        public Guid ProgrammeId { get; set; }
        public string ProgrammeProcessId { get; set; }
        public MilestoneTemplateVM MilestoneTemplate { get; set; }
        public IEnumerable<SelectListItem> EmailAddresses { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }
        public AdvisoryVM AdvisoryContent { get; set; }        
        public EmailTemplateVM EmailTemplate { get; set; }
        public UserTaskVM UserTask { get; set; }
        public string ActivityId { get; set; }        
    }

    public class EmailTemplateVM
    {
        public string EmailAdress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MilestoneTemplateVM
    {
        public IList<Activity> Activities { get; set; }
        public IList<ProgrammeProcess> ProgrammeProcesses { get; set; }
    }

    public class AdvisoryVM
    {
        public string Advisory { get; set; }
    }

    public class UserTaskVM
    {
        public int Priority { get; set; }
        public string Details { get; set; }
        public string Description { get; set; }
        public DateTime DueDate {get; set;}
    }

}

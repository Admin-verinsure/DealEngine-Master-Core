using System.Collections.Generic;
using TechCertain.Domain.Entities;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TechCertain.WebUI.Models.Milestone
{
    public class MilestoneBuilderViewModel : BaseViewModel
    {
        public Guid ProgrammeId { get; set; }
        public IList<string> Actions { get; set; }  
        public IList<EmailTemplate> EmailTemplates { get; set; }
        public IList<UserTask> UserTasks { get; set; }
        public IList<string> Advisories { get; set; }
        public IList<string> EmailAddresses { get; set; }
        public IEnumerable<SelectListItem> Priorities { get; set; }
        public AdvisoryVM AdvisoryContent { get; set; }        
        public EmailTemplateVM EmailTemplate { get; set; }
        public UserTaskVM UserTask { get; set; }
        public string Type { get; set; }


        //New setup 
        public MilestoneTemplateVM MilestoneTemplate { get; set; }

    }

    public class EmailTemplateVM
    {
        public string EmailAdress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class MilestoneTemplateVM
    {
        public string Activity { get; set; }
        public IList<string> Templates { get; set; }
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

using DealEngine.Domain.Entities;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace DealEngine.WebUI.Models.Information
{
    public class MoveAdvisorsViewModel : BaseViewModel
    {
        public Guid id { get; set; }
        // This should be what you're actually displaying
        [Display(Name = "List of Advisors")]
        public IList<Domain.Entities.Organisation> advisors { get; set; }
        //public List
        public MoveAdvisorsViewModel(Guid clientProgrammeId, IList<Domain.Entities.Organisation> advisors)
        {
            id = clientProgrammeId;
            PopulateAdvisorList(advisors);
        }
        private void PopulateAdvisorList(IList<Domain.Entities.Organisation> advisors)
        {

        }
    }
}
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using DealEngine.Domain.Entities;

namespace DealEngine.WebUI.Models.Programme
{

    public class ActivityListViewModel : BaseViewModel
    {
        public IList<Domain.Entities.Programme> ProgrammeList { get; set; }
        public IList<BusinessActivityTemplate> BusinessActivityList { get; set; }

    };

    
}


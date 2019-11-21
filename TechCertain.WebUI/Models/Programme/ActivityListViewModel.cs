using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Programme
{

    public class ActivityListViewModel : BaseViewModel
    {
        public IList<Domain.Entities.Programme> ProgrammeList { get; set; }
        public IList<BusinessActivity> BusinessActivityList { get; set; }

    };

    
}


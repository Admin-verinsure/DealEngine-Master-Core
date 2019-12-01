using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.WebUI.Models.Programme
{

    public class ActivityViewModel : BaseViewModel
    {
        public ActivityBuilderVM Builder { get; set; }
        public ActivityAttachVM ActivityAttach { get; set; }
        public ActivityModal ActivityCreate { get; set; }
        public ActivityListViewModel ActivityListViewModel { get; set; }
        public Guid Id { get; set; }
    };

    public class ActivityBuilderVM
    {
        public IList<BusinessActivityTemplate> Level1Classifications { get; set; }
        public IList<BusinessActivityTemplate> Level2Classifications { get; set; }
        public IList<BusinessActivityTemplate> Level3Classifications { get; set; }
        public IList<BusinessActivityTemplate> Level4Classifications { get; set; }
        public IList<SelectListItem> Activities { get; set; }
        public string[] SelectedActivities { get; set; }
        public bool Ispublic { get; set; }
    }

    public class ActivityAttachVM
    {
        public string[] SelectedProgramme { get; set; }
        public IList<SelectListItem> BaseProgList { get; set; }
    }

    public class ActivityModal
    {
        public BusinessActivityViewModel ClassOne { get; set; }
        public BusinessActivityViewModel ClassTwo { get; set; }
        public BusinessActivityViewModel ClassThree { get; set; }
        public BusinessActivityViewModel ClassFour { get; set; }
    }

    
}


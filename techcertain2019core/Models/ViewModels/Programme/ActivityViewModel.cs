using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace techcertain2019core.Models.ViewModels.Programme
{

    public class ActivityViewModel : BaseViewModel
    {
        public ActivityBuilderVM Builder { get; set; }
        public ActivityAttachVM ActivityAttach { get; set; }
        public ActivityModal ActivityCreate { get; set; }

    };

    public class ActivityBuilderVM
    {
        public IEnumerable<BusinessActivityViewModel> Level1Classifications { get; set; }
        public IEnumerable<BusinessActivityViewModel> Level2Classifications { get; set; }
        public IEnumerable<BusinessActivityViewModel> Level3Classifications { get; set; }
        public IEnumerable<BusinessActivityViewModel> Level4Classifications { get; set; }
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


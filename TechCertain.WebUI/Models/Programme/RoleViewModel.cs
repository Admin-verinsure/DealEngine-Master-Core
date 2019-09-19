using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace TechCertain.WebUI.Models.Programme
{

    public class RoleViewModel : BaseViewModel
    {
        public RoleBuilderVM Builder { get; set; }
        public RoleAttachVM RoleAttach { get; set; }

    };

    public class RoleBuilderVM
    {
        public IList<SelectListItem> Roles { get; set; }
        public string[] SelectedRoles { get; set; }
        public bool Ispublic { get; set; }
    }

    public class RoleAttachVM
    {
        public string[] SelectedProgramme { get; set; }
        public IList<SelectListItem> BaseProgList { get; set; }
    }

}


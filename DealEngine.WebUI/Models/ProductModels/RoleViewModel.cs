using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.ProductModels
{

    public class RoleViewModel : BaseViewModel
    {
        public RoleBuilderVM Builder { get; set; }
        public RoleAttachVM RoleAttach { get; set; }
    };

    public class RoleBuilderVM
    {
        public string Role { get; set; }
    }

    public class RoleAttachVM
    {
        public string[] SelectedProgramme { get; set; }
        public IList<SelectListItem> BaseProgList { get; set; }
    }



}


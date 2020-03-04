﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace DealEngine.WebUI.Models.Programme
{

    public class SharedRoleTemplateViewModel : BaseViewModel
    {
        public IList<SelectListItem> BaseProgList { get; set; }
        public IList<SelectListItem> Roles { get; set; }
    }

}


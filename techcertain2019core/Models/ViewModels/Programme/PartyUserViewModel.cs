﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using TechCertain.Domain.Entities;

namespace techcertain2019core.Models.ViewModels.Programme
{


	public class PartyUserViewModel : BaseViewModel
	{
		public string Id { get; set; }
        
        public string Name { get; set; }
        public string Email { get; set; }


    }
}


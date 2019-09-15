using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace techcertain2015rebuildcore.Controllers
{
    public class CustomerController : Microsoft.AspNetCore.Mvc.Controller
    {
		// GET: Customer
		[HttpGet]
        public ActionResult Parties()
        {
            return View();
        }
    }
}
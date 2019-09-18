using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace techcertain2019core.Controllers
{
    public class CustomerController : Controller
    {
		// GET: Customer
		[HttpGet]
        public ActionResult Parties()
        {
            return View();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechCertain.WebUI.Controllers
{
    public class CustomerController : Controller
    {
		// GET: Customer
		[HttpGet]
        public IActionResult Parties()
        {
            return View();
        }
    }
}
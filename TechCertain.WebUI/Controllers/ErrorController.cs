using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Services.Interfaces;

namespace TechCertain.WebUI
{
    public class ErrorController : Controller
    {
        ICilentInformationService _clientInformationService;
        // GET: Error
        [ActionName ("Error403")]
		[HttpGet]
		public ActionResult Error403 ()
		{
            Response.StatusCode = 403;
			return View ();
		}

		// GET: Error
		[ActionName("Error404")]
		[HttpGet]
        public ActionResult Error404 ()
        {
            Response.StatusCode = 404;
            return View();
        }

        // GET: Error
		[ActionName("Error500")]
		[HttpGet]
        public ActionResult Error500 ()
        {
            Response.StatusCode = 500;
            return View();
        }

		[HttpGet]
		public ViewResult Antiforgery ()
		{
            return View();
		}

		[HttpGet]
		public ViewResult InvalidToken ()
		{
            return View ();
		}

		[HttpGet]
		public ViewResult InvalidPasswordReset ()
		{
            return View ();
		}

		[HttpGet]
		public ActionResult DangerousRequest ()
		{
            Response.StatusCode = 400;
            return View();
        }

        [HttpGet]
        public ActionResult PaymentRequestError()
        {
            //"bc3c9972 - 1733 - 41a1 - 8786 - fa22229c66f8"
            //Console.WriteLine(Id);
            //ClientInformationSheet sheet = _clientInformationService.GetInformation(Id);
            //InformationViewModel model = new InformationViewModel();
            //Response.StatusCode = 400;

            return View();
        }

    }
}
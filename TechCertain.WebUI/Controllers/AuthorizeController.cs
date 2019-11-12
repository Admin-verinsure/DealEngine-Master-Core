using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechCertain.Services.Interfaces;

namespace TechCertain.WebUI.Controllers
{
    public class AuthorizeController : BaseController
    {
        public AuthorizeController(IUserService userService)
            : base(userService)
        {

        }

        // GET: Authorize
        public ActionResult Index()
        {
            return View();
        }

        // GET: Authorize/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Authorize/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Authorize/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Authorize/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Authorize/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Authorize/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Authorize/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
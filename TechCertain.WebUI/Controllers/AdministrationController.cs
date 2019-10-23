using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Permission;

namespace TechCertain.WebUI.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        //[HttpGet]
        //public IActionResult CreateRole()
        //{
        //    return View();
        //}


        [HttpGet]
        public ActionResult Index()
        {
            var models = new BaseListViewModel<GroupViewModel>();
            //foreach (var group in GetAllGroups())
            //{
            //    models.Add(new GroupViewModel(group));
            //}

            return View(models);
        }


    }
}
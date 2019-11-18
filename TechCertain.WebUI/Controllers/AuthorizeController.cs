using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHibernate.AspNetCore.Identity;
using NHibernate.Linq;
using TechCertain.Services.Interfaces;
using TechCertain.WebUI.Models.Authorization;
using IdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;
using IdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;

namespace TechCertain.WebUI.Controllers
{
    public class AuthorizeController : BaseController
    {
        IClaimService _claimService;
        IClaimTemplateService _claimTemplateService;
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;

        public AuthorizeController(IUserService userService, IClaimService claimService, IClaimTemplateService claimTemplateService, 
            RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
            : base(userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _claimService = claimService;
            _claimTemplateService = claimTemplateService;
        }

        // GET: Authorize
        public async Task<IActionResult> Index()
        {
            var roleList = await _roleManager.Roles.ToListAsync();
            var userList = await _userManager.Users.ToListAsync();
            var user = await CurrentUser();
            AuthorizeViewModel model = new AuthorizeViewModel();

            var claimList = await _claimService.GetClaimsByOrganisation(user.PrimaryOrganisation);
            if (claimList.Count == 0)
            {
                await _claimTemplateService.CreateClaimsForOrganisation(user.PrimaryOrganisation);
                claimList = await _claimService.GetClaimsByOrganisation(user.PrimaryOrganisation);
            }
            model.RoleList = new List<IdentityRole>();
            model.UserList = new List<IdentityUser>();
            model.ClaimList = claimList;

            if (roleList.Count != 0)
            {
                model.RoleList = roleList;
            }

            if (userList.Count != 0)
            {
                model.UserList = userList;
            }

            return View(model);            
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string RoleName, string[] Claims)
        {
            var isRole = await _roleManager.RoleExistsAsync(RoleName);
            if (!isRole)
            {
                var role = new IdentityRole
                {
                    Name = RoleName
                };

                var identityreult = await _roleManager.CreateAsync(role);
                if (identityreult.Succeeded)
                {
                    foreach(var cl in Claims)
                    {
                        var claim = new Claim(cl, cl);
                        await _roleManager.AddClaimAsync(role, claim);
                    }
                    return Ok();
                }
            }

            return Ok();            
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string RoleName, string[] Claims)
        {
            var isRole = await _roleManager.RoleExistsAsync(RoleName);
            if (isRole)
            {
                var role = await _roleManager.FindByNameAsync(RoleName);
                var claimList = await _roleManager.GetClaimsAsync(role);

                foreach (var claim in claimList)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }

                foreach (var cl in Claims)
                {
                    var template = await _claimService.GetTemplateByName(cl);
                    var claim = new Claim(template.Type, template.Value);
                    await _roleManager.AddClaimAsync(role, claim);
                }

                return Ok();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SaveRoleToUser(string UserId, string[] RoleIds)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if(user != null)
            {
                foreach(var id in RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    if(role != null)
                    {
                        await _userManager.AddToRoleAsync(user, role.Name);
                    }
                }
            }

            return Ok();
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
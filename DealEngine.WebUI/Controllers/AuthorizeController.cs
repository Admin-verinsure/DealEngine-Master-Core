using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DealEngine.Domain.Entities;
using DealEngine.Services.Interfaces;
using DealEngine.WebUI.Models.Authorization;
using IdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;
using IdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;
using Claim = System.Security.Claims.Claim;
using NHibernate.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace DealEngine.WebUI.Controllers
{
    public class AuthorizeController : BaseController
    {
        IClaimService _claimService;
        IClaimTemplateService _claimTemplateService;
        IUserRoleService _userRoleService;
        RoleManager<IdentityRole> _roleManager;
        UserManager<IdentityUser> _userManager;
        IOrganisationService _organisationService;
        IApplicationLoggingService _applicationLoggingService;
        ILogger<AuthorizeController> _logger;

        public AuthorizeController(
            IUserService userService, 
            IClaimService claimService, 
            IClaimTemplateService claimTemplateService, 
            RoleManager<IdentityRole> roleManager, 
            UserManager<IdentityUser> userManager, 
            IUserRoleService userRoleService, 
            IOrganisationService organisationService,
            IApplicationLoggingService applicationLoggingService,
            ILogger<AuthorizeController> logger
            )
            : base(userService)
        {
            _logger = logger;
            _applicationLoggingService = applicationLoggingService;
            _organisationService = organisationService;
            _userRoleService = userRoleService;
            _userManager = userManager;
            _roleManager = roleManager;
            _claimService = claimService;
            _claimTemplateService = claimTemplateService;
            _userService = userService;
        }

        // GET: Authorize
        public async Task<IActionResult> Index()
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                var userRoleList = await _userRoleService.GetRolesByOrganisation(user.PrimaryOrganisation);

                var userList = await _userService.GetAllUsers();
                var roleList = new List<IdentityRole>();
                var organisationList = await _organisationService.GetAllOrganisations();

                var claimList = await _claimService.GetClaimsAllClaimsList();
                if (claimList.Count == 0)
                {
                    await _claimTemplateService.CreateAllClaims();
                    claimList = await _claimService.GetClaimsAllClaimsList();
                }

                AuthorizeViewModel model = new AuthorizeViewModel();

                model.RoleList = new List<IdentityRole>();
                model.UserList = new List<User>();
                model.ClaimList = claimList;
                model.Organisations = organisationList;

                if (user.PrimaryOrganisation.IsTC)
                {
                    roleList = await _roleManager.Roles.ToListAsync();
                }
                else
                {
                    if (userRoleList.Count != 0)
                    {
                        foreach (var userRole in userRoleList)
                        {
                            var identityRole = await _roleManager.FindByNameAsync(userRole.IdentityRoleName);
                            roleList.Add(identityRole);
                        }

                    }
                }

                model.RoleList = roleList;

                if (userList.Count != 0)
                {
                    model.UserList = userList;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(string RoleName, string[] Claims, string OrganisationId)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
                var isRole = await _roleManager.RoleExistsAsync(RoleName);
                var organisation = user.PrimaryOrganisation;

                if (OrganisationId != null)
                {
                    organisation = await _organisationService.GetOrganisation(Guid.Parse(OrganisationId));
                }

                if (!isRole)
                {
                    var role = new IdentityRole
                    {
                        Name = RoleName
                    };

                    var identityreult = await _roleManager.CreateAsync(role);
                    if (identityreult.Succeeded)
                    {
                        await _userRoleService.AddUserRole(user, role, organisation);

                        foreach (var cl in Claims)
                        {
                            var claim = new Claim(cl, cl);
                            await _roleManager.AddClaimAsync(role, claim);
                        }
                        return Ok();
                    }
                }

                return Ok();
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
                     
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string RoleName, string[] Claims)
        {
            User user = null;
            try
            {
                user = await CurrentUser();
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
                        var claim = new Claim(template.Value, template.Value);
                        await _roleManager.AddClaimAsync(role, claim);
                    }

                    return Ok();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRoleToUser(Guid UserId, string[] RoleIds)
        {
            User user = null;
            try
            {
                user = await _userService.GetUserById(UserId);
                var identityUser = await _userManager.FindByNameAsync(user.UserName);
                if (identityUser == null)
                {
                    identityUser = new IdentityUser();
                    identityUser.UserName = user.UserName;
                    identityUser.Email = user.Email;
                    await _userManager.CreateAsync(identityUser);
                }

                foreach (var id in RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(id);
                    if (role != null)
                    {
                        await _userManager.AddToRoleAsync(identityUser, role.Name);
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }

        }

        [HttpPost]
        public async Task<IActionResult> CreateClaim(IFormCollection form)
        {
            User user = null;
            try
            {
                var claimvalue = form["value"];
                var claimtype = form["type"];
                Domain.Entities.Claim claim = new Domain.Entities.Claim(claimtype, claimvalue);
                await _claimService.AddClaim(claim);                
            }
            catch(Exception ex)
            {
                await _applicationLoggingService.LogWarning(_logger, ex, user, HttpContext);
                return RedirectToAction("Error500", "Error");
            }
                return NoContent();
        }
    }
}
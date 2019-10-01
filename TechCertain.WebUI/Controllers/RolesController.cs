﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using TechCertain.WebUI.Areas.Identity.Data;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Permission;

namespace TechCertain.WebUI.Controllers
{
    public class RolesController : BaseController
    {
		IRolePermissionsService _roleService;
		ILogger _logger;

		public RolesController (IUserService userService, DealEngineDBContext dealEngineDBContext, IRolePermissionsService rolePermissionsService, ILogger logger)
			: base (userService, dealEngineDBContext)
		{
			_roleService = rolePermissionsService;
			_logger = logger;
		}

		[HttpGet]
		public ActionResult Index ()
		{
			var models = new BaseListViewModel<RoleViewModel> ();
			foreach (var role in _roleService.GetAllRoles()) {
				models.Add (new RoleViewModel (role));
			}

			return View (models);
		}

		public ActionResult Details (int id)
		{
			return View ();
		}

		[HttpPost]
		public ActionResult Create (RoleViewModel roleViewModel)
		{
			try {
                throw new Exception("Method will need to be re-written");
                //if (!ModelState.IsValidField(nameof(roleViewModel.RoleName)))
                //	return RedirectToAction ("Index");

                ApplicationRole appRole = _roleService.CreateRole (roleViewModel.RoleName);
				appRole.Description = roleViewModel.Description;
				_roleService.UpdateRole (appRole);

				return RedirectToAction ("Index");
			}
			catch {
				return RedirectToAction ("Index");
			}
		}

		[HttpPost]
		public ActionResult Edit (RoleViewModel roleViewModel)
		{
			try {
				ApplicationRole appRole = _roleService.GetRole (roleViewModel.Id);
				if (appRole == null)
					throw new Exception ("Unable to find ApplicationRole with Id [" + roleViewModel.Id + "]");

				appRole.Name = roleViewModel.RoleName;
				appRole.Description = roleViewModel.Description;
				_roleService.UpdateRole (appRole);

				return Json (true);
			}
			catch (Exception ex) {
				_logger.Error (ex);
                throw new Exception("Method will need to be re-written");
				//return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, "Unable to save role with Id: [" + roleViewModel.Id + "]");
			}
		}

		[HttpPost]
		public ActionResult Delete (Guid Id)
		{
			try {
				if (!_roleService.DeleteRole (Id, CurrentUser))
					throw new Exception ("Unable to delete ApplicationRole with Id [" + Id + "]");

				return Json (true);
			}
			catch (Exception ex) {
				_logger.Error (ex);
                throw new Exception("Method will need to be re-written");
                //return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, ex.Message);
			}
		}
    }
}
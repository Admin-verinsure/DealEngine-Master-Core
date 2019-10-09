using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;
using DealEngine.Infrastructure.Identity.Data;
using TechCertain.WebUI.Models;
using TechCertain.WebUI.Models.Permission;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace TechCertain.WebUI.Controllers
{
    public class GroupsController : BaseController
    {
		/**
		 * References
		 * 
		 * http://johnatten.com/2014/02/19/asp-net-mvc-5-identity-implementing-group-based-permissions-management-part-ii/
		 * https://github.com/TypecastException/AspNetGroupBasedPermissions
		 */

		IRolePermissionsService _roleService;
        IHttpContextAccessor httpContextAccessor;
        public GroupsController (IUserService userService,
            SignInManager<DealEngineUser> signInManager,
            IHttpContextAccessor httpContextAccessor, 
            DealEngineDBContext dealEngineDBContext, 
            IRolePermissionsService rolePermissionsService)
			: base(userService, dealEngineDBContext, signInManager, httpContextAccessor)
		{
			_roleService = rolePermissionsService;			
		}

		[HttpGet]
        public ActionResult Index()
        {
			var models = new BaseListViewModel<GroupViewModel>();
			foreach (var group in GetAllGroups ()) {
				models.Add (new GroupViewModel (group));
			}

			return View (models);
        }

		[HttpGet]
        public ActionResult Details(Guid id)
        {
            return View ();
        }

		[HttpGet]
        public ActionResult Create()
        {
            return View ();
        } 

        [HttpPost]
		public ActionResult Create(GroupViewModel model)
        {
            if (ModelState.IsValid) {
				_roleService.CreateGroup (model.Name);
				return RedirectToAction ("Index");
			}
			return View (model);
        }
        
		[HttpGet]
        public ActionResult Edit(Guid id)
        {
			ApplicationGroup group = GetGroup (id);
            if (group == null)
                throw new Exception("Method will need to be re-written");
				//return HttpNotFound ();

			return View (new GroupViewModel(group));
        }

        [HttpPost]
        public ActionResult Edit(Guid id, GroupViewModel group)
        {
            try {
                return RedirectToAction ("Index");
            } catch {
                return View ();
            }
        }

		[HttpGet]
        public ActionResult Delete(Guid id)
        {
			if (id == Guid.Empty) {
                throw new Exception("Method will need to be re-written");
                //return new HttpStatusCodeResult (HttpStatusCode.BadRequest);
			}
			ApplicationGroup group = GetGroup (id);
			if (group == null) {
                throw new Exception("Method will need to be re-written");
                //return HttpNotFound ();
			}
			return View (new GroupViewModel (group));
        }

        [HttpPost, ActionName ("Delete")]
        public ActionResult DeleteConfirmed (Guid id)
        {
            try {
				if (!_roleService.DeleteGroup (id, CurrentUser))
					throw new Exception ("Unable to delete ApplicationGroup with Id [" + id + "]");

				return Json (true);
            }
			catch (Exception ex) {				
                throw new Exception("Method will need to be re-written " + ex.Message);
                //return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, ex.Message);
            }
        }

		[HttpGet]
		public ActionResult GroupRoles (Guid id)
		{
			ApplicationGroup group = GetGroup (id);
			var model = new SelectGroupRolesViewModel ();
			model.GroupId = group.Id;
			model.GroupName = group.Name;
			foreach (var role in GetAllRoles()) {
				var evm = new SelectRoleEditorViewModel (role);
				evm.Selected = false;
				model.Roles.Add (evm);
			}
			foreach (var groupRole in group.Roles) {
				var selectedRole = model.Roles.FirstOrDefault (r => r.RoleId == groupRole.Id);
				if (selectedRole != null)
					selectedRole.Selected = true;
			}

			return View (model);
		}

		[HttpPost]
		public ActionResult GroupRoles (SelectGroupRolesViewModel model)
		{
			try {
				List<Guid> selectedRoleIds = new List<Guid> ();
				List<Guid> unselectedRoleIds = new List<Guid> ();

				foreach (var roleModel in model.Roles)
					if (roleModel.Selected)
						selectedRoleIds.Add (roleModel.RoleId);
					else
						unselectedRoleIds.Add (roleModel.RoleId);

				_roleService.RemoveRolesFromGroup (unselectedRoleIds.ToArray(), model.GroupId);
				_roleService.AddRolesToGroup (selectedRoleIds.ToArray(), model.GroupId);

				return RedirectToAction ("Index");
			}
			catch (Exception ex) {
				Elmah.ErrorSignal.FromCurrentContext ().Raise (ex);
				ModelState.AddModelError ("", "Unable to modifiy roles for the given group.");
				return View (model);
			}
		}

		[HttpPost]
		public ActionResult SetUserGroups (SelectUserGroupsViewModel model)
		{
			List<Guid> selectedGroupIds = new List<Guid> ();
			List<Guid> unselectedGroupIds = new List<Guid> ();

			foreach (var groupModel in model.Groups)
				if (groupModel.Selected)
					selectedGroupIds.Add (groupModel.GroupId);
				else
					unselectedGroupIds.Add (groupModel.GroupId);

			_roleService.AddUserToGroups (model.UserId, selectedGroupIds.ToArray ());
			_roleService.RemoveUserFromGroups (model.UserId, unselectedGroupIds.ToArray ());

			return RedirectToAction ("ManageUser", "Account", new { id = model.UserId });
		}

		IEnumerable<ApplicationRole> GetAllRoles ()
		{
			return _roleService.GetAllRoles ();
		}

		IEnumerable<ApplicationGroup> GetAllGroups ()
		{
			return _roleService.GetAllGroups ();
		}

		ApplicationGroup GetGroup (Guid groupId)
		{
			return GetAllGroups ().FirstOrDefault (g => g.Id == groupId);
		}

		///// <summary>
		///// Checks to make sure that the current user belongs to one of the specified roles, otherwise a 403 Forbidden error is raised.
		///// </summary>
		///// <param name="roles">List of possible Roles the user can belong to.</param>
		//protected void AuthorizeRoles (params string [] roles)
		//{
		//	bool hasAccess = false;
		//	foreach (var role in roles) {
		//		if (_roleService.DoesUserHaveRole (CurrentUser.UserName, role)) {
		//			hasAccess = true;
		//			break;
		//		}
		//	}
		//	if (!hasAccess)
		//		throw new HttpException ((int)HttpStatusCode.Forbidden, "Forbidden");
		//}

		///// <summary>
		///// Checks to make sure the current user has the Admin role, otherwise raises a 403 Forbidden error.
		///// </summary>
		//protected void AuthorizeAdminOnly ()
		//{
		//	AuthorizeRoles ("Admin");
		//}
    }
}
 
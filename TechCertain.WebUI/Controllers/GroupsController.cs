//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using TechCertain.Domain.Entities;
//using TechCertain.Services.Interfaces;
//using TechCertain.WebUI.Models;
//using TechCertain.WebUI.Models.Permission;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using System.Threading.Tasks;

//namespace TechCertain.WebUI.Controllers
//{
//    public class GroupsController : BaseController 
//    {
//		/**
//		 * References
//		 * 
//		 * http://johnatten.com/2014/02/19/asp-net-mvc-5-identity-implementing-group-based-permissions-management-part-ii/
//		 * https://github.com/TypecastException/AspNetGroupBasedPermissions
//		 */

//		//IRolePermissionsService _roleService;
//        RoleManager<IdentityRole> _roleManager;
//        UserManager<DealEngineUser> _userManager;
//        IClaimService _ClaimService;

//<<<<<<< HEAD
//		[HttpGet]
//        public async Task<IActionResult> Index()
//=======


//        //private readonly RoleManager<IdentityRole> roleManager;

//        //public GroupsController(RoleManager<IdentityRole> roleManager)
//        //{
//        //    this.roleManager = roleManager;
//        //}

//        public GroupsController(IUserService userService,
//            UserManager<DealEngineUser> userManager,
//            IClaimService claimService,
//            RoleManager<IdentityRole> roleManager)
//            : base(userService)
//>>>>>>> techcertain2019coreIdentity
//        {
//            //_roleService = rolePermissionsService;
//            _ClaimService = claimService;
//            _roleManager = roleManager;
//            _userManager = userManager;
//            //_roleClaims = roleClaims;

//        }

//<<<<<<< HEAD
//		[HttpGet]
//        public async Task<IActionResult> Details(Guid id)
//=======
//        [HttpGet]
//        public IActionResult Index()
//>>>>>>> techcertain2019coreIdentity
//        {
//			var models = new BaseListViewModel<GroupViewModel>();
//            foreach (var group in _roleManager.Roles)
//            {
//                models.Add(new GroupViewModel(group));
//            }
//            return View (models);
//        }

//<<<<<<< HEAD
//		[HttpGet]
//        public async Task<IActionResult> Create()
//        {
//            return View ();
//        } 

//        [HttpPost]
//		public async Task<IActionResult> Create(GroupViewModel model)
//        {
//            if (ModelState.IsValid) {
//				_roleService.CreateGroup (model.Name);
//				return RedirectToAction ("Index");
//			}
//			return View (model);
//        }
        
//		[HttpGet]
//        public async Task<IActionResult> Edit(Guid id)
//        {
//			ApplicationGroup group = GetGroup (id);
//            if (group == null)
//                throw new Exception("Method will need to be re-written");
//				//return HttpNotFound ();
//=======
//        //[HttpGet]
//        //public async Task<IActionResult> Create()
//        //{
//        //    return View ();
//        //}


//        //[HttpGet]
//        //public IActionResult GetClaim()
//        //{
//        //    var models = new BaseListViewModel<GroupViewModel>();

//        //    //foreach (var claim in _roleClaims.GetClaimsAsync<System.Security.Claims.Claim>)
//        //    //{
//        //    //    models.Add(new GroupViewModel(group));
//        //    //}

//        //    return View(models);
//        //}


//        [HttpPost]
//        public async Task<IActionResult> CreateClaim(string Name)
//        {
//            if (ModelState.IsValid)
//            {
//                //_roleService.CreateGroup (model.Name);
//                IdentityRole identityRole = new IdentityRole
//                {
//                    Name = Name

//                };
//                System.Security.Claims.Claim cl = new System.Security.Claims.Claim(Name, Name);
//                //await _roleClaims.AddClaimAsync(identityRole, cl);
//>>>>>>> techcertain2019coreIdentity


//            }
//            return View();
//        }



//        [HttpPost]
//<<<<<<< HEAD
//        public async Task<IActionResult> Edit(Guid id, GroupViewModel group)
//=======
//		public async Task<IActionResult> Index(string Name)
//>>>>>>> techcertain2019coreIdentity
//        {
//            if (ModelState.IsValid) {
//                //_roleService.CreateGroup (model.Name);
//                IdentityRole identityRole = new IdentityRole
//                {
//                    Name = Name

//                };
//                IdentityResult result = await _roleManager.CreateAsync(identityRole);

//                if(result.Succeeded == true)
//                {
//                    return RedirectToAction("Index", "Home");
//                }
//                foreach(IdentityError error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            return View();
//        }

//<<<<<<< HEAD
//		[HttpGet]
//        public async Task<IActionResult> Delete(Guid id)
//=======
//        //[HttpGet]
//        //      public ActionResult Edit(Guid id)
//        //      {
//        //	ApplicationGroup group = GetGroup (id);
//        //          if (group == null)
//        //              throw new Exception("Method will need to be re-written");
//        //		//return HttpNotFound ();

//        //	return View (new GroupViewModel(group));
//        //      }

//        //[HttpPost]
//        //public async Task<IActionResult> Edit(Guid id, GroupViewModel group)
//        //{
//        //    try {
//        //        return RedirectToAction ("Index");
//        //    } catch {
//        //        return View ();
//        //    }
//        //}

//        [HttpGet]
//        public async Task<IActionResult> Delete(string id)
//>>>>>>> techcertain2019coreIdentity
//        {
//            if (id == null)
//            {
//                throw new Exception("Method will need to be re-written");
//                //return new HttpStatusCodeResult (HttpStatusCode.BadRequest);
//            }
//            var role =  await _roleManager.FindByIdAsync(id);

//            if (role == null)
//            {
//                throw new Exception("Method will need to be re-written");
//                //return HttpNotFound ();
//            }
//            else
//            {
//                var result = await _roleManager.DeleteAsync(role);

//                if (result.Succeeded == true)
//                {
//                    return RedirectToAction("Index", "Home");
//                }
//                foreach (IdentityError error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            return View("Index");
//        }

//<<<<<<< HEAD
//        [HttpPost, ActionName ("Delete")]
//        public async Task<IActionResult> DeleteConfirmed (Guid id)
//=======
//        //     [HttpPost, ActionName ("Delete")]
//        //     public ActionResult DeleteConfirmed (Guid id)
//        //     {
//        //         try {
//        //	if (!_roleService.DeleteGroup (id, CurrentUser))
//        //		throw new Exception ("Unable to delete ApplicationGroup with Id [" + id + "]");

//        //	return Json (true);
//        //         }
//        //catch (Exception ex) {				
//        //             throw new Exception("Method will need to be re-written " + ex.Message);
//        //             //return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, ex.Message);
//        //         }
//        //     }

//        [HttpGet]
//        public async Task<ActionResult> GroupRoles(string id)
//>>>>>>> techcertain2019coreIdentity
//        {
//            IdentityRole identityRole = await _roleManager.FindByIdAsync(id);

//            var model = new SelectClaimsRolesViewModel();
//            model.GroupId = Guid.Parse(identityRole.Id);
//            model.GroupName = identityRole.Name;
//            foreach (var claim in GetAllClaims())
//            {
//                var evm = new SelectClaimEditorViewModel(claim);
//                evm.Selected = false;
//                model.Claims.Add(evm);
//            }
//            //foreach (var groupclaim in identityRole..Claims)
//            //{
//            //    var selectedRole = model.Claims.FirstOrDefault(r => r.ClaimId == groupclaim.Id);
//            //    if (selectedRole != null)
//            //        selectedRole.Selected = true;
//            //}

//            return View(model);
//        }
//        AuthClaims GetClaim(Guid groupId)
//        {
//            return GetAllClaims().FirstOrDefault(g => g.Id == groupId);
//        }

//        IEnumerable<AuthClaims> GetAllClaims()
//        {
//            return _ClaimService.GetAllClaims();
//        }

//        //[HttpPost]
//        //public ActionResult GroupRoles(SelectGroupRolesViewModel model)
//        //{
//        //    try
//        //    {
//        //        List<Guid> selectedRoleIds = new List<Guid>();
//        //        List<Guid> unselectedRoleIds = new List<Guid>();

//        //        foreach (var roleModel in model.Roles)
//        //            if (roleModel.Selected)
//        //                selectedRoleIds.Add(roleModel.RoleId);
//        //            else
//        //                unselectedRoleIds.Add(roleModel.RoleId);

//        //        _ClaimService.AddClaimAsync.RemoveRolesFromGroup(unselectedRoleIds.ToArray(), model.GroupId);
//        //        _roleService.AddRolesToGroup(selectedRoleIds.ToArray(), model.GroupId);

//        //        return RedirectToAction("Index");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
//        //        ModelState.AddModelError("", "Unable to modifiy roles for the given group.");
//        //        return View(model);
//        //    }
//        //}

//        //[HttpPost]
//        //public ActionResult SetUserGroups (SelectUserGroupsViewModel model)
//        //{
//        //	List<Guid> selectedGroupIds = new List<Guid> ();
//        //	List<Guid> unselectedGroupIds = new List<Guid> ();

//        //	foreach (var groupModel in model.Groups)
//        //		if (groupModel.Selected)
//        //			selectedGroupIds.Add (groupModel.GroupId);
//        //		else
//        //			unselectedGroupIds.Add (groupModel.GroupId);

//        //	_roleService.AddUserToGroups (model.UserId, selectedGroupIds.ToArray ());
//        //	_roleService.RemoveUserFromGroups (model.UserId, unselectedGroupIds.ToArray ());

//        //	return RedirectToAction ("ManageUser", "Account", new { id = model.UserId });
//        //}

//        //IEnumerable<ApplicationRole> GetAllRoles ()
//        //{
//        //	return _roleService.GetAllRoles ();
//        //}

//        //IEnumerable<ApplicationGroup> GetAllGroups ()
//        //{
//        //	return _roleService.GetAllGroups ();
//        //}

//        //ApplicationGroup GetGroup (Guid groupId)
//        //{
//        //	return GetAllGroups ().FirstOrDefault (g => g.Id == groupId);
//        //}

//        ///// <summary>
//        ///// Checks to make sure that the current user belongs to one of the specified roles, otherwise a 403 Forbidden error is raised.
//        ///// </summary>
//        ///// <param name="roles">List of possible Roles the user can belong to.</param>
//        //protected void AuthorizeRoles (params string [] roles)
//        //{
//        //	bool hasAccess = false;
//        //	foreach (var role in roles) {
//        //		if (_roleService.DoesUserHaveRole (CurrentUser.UserName, role)) {
//        //			hasAccess = true;
//        //			break;
//        //		}
//        //	}
//        //	if (!hasAccess)
//        //		throw new HttpException ((int)HttpStatusCode.Forbidden, "Forbidden");
//        //}

//<<<<<<< HEAD
//		[HttpGet]
//		public async Task<IActionResult> GroupRoles (Guid id)
//		{
//			ApplicationGroup group = GetGroup (id);
//			var model = new SelectGroupRolesViewModel ();
//			model.GroupId = group.Id;
//			model.GroupName = group.Name;
//			foreach (var role in GetAllRoles()) {
//				var evm = new SelectRoleEditorViewModel (role);
//				evm.Selected = false;
//				model.Roles.Add (evm);
//			}
//			foreach (var groupRole in group.Roles) {
//				var selectedRole = model.Roles.FirstOrDefault (r => r.RoleId == groupRole.Id);
//				if (selectedRole != null)
//					selectedRole.Selected = true;
//			}

//			return View (model);
//		}

//		[HttpPost]
//		public async Task<IActionResult> GroupRoles (SelectGroupRolesViewModel model)
//		{
//			try {
//				List<Guid> selectedRoleIds = new List<Guid> ();
//				List<Guid> unselectedRoleIds = new List<Guid> ();

//				foreach (var roleModel in model.Roles)
//					if (roleModel.Selected)
//						selectedRoleIds.Add (roleModel.RoleId);
//					else
//						unselectedRoleIds.Add (roleModel.RoleId);

//				_roleService.RemoveRolesFromGroup (unselectedRoleIds.ToArray(), model.GroupId);
//				_roleService.AddRolesToGroup (selectedRoleIds.ToArray(), model.GroupId);

//				return RedirectToAction ("Index");
//			}
//			catch (Exception ex) {
//				Elmah.ErrorSignal.FromCurrentContext ().Raise (ex);
//				ModelState.AddModelError ("", "Unable to modifiy roles for the given group.");
//				return View (model);
//			}
//		}

//		[HttpPost]
//		public async Task<IActionResult> SetUserGroups (SelectUserGroupsViewModel model)
//		{
//			List<Guid> selectedGroupIds = new List<Guid> ();
//			List<Guid> unselectedGroupIds = new List<Guid> ();

//			foreach (var groupModel in model.Groups)
//				if (groupModel.Selected)
//					selectedGroupIds.Add (groupModel.GroupId);
//				else
//					unselectedGroupIds.Add (groupModel.GroupId);

//			_roleService.AddUserToGroups (model.UserId, selectedGroupIds.ToArray ());
//			_roleService.RemoveUserFromGroups (model.UserId, unselectedGroupIds.ToArray ());

//			return RedirectToAction ("ManageUser", "Account", new { id = model.UserId });
//		}

//		IEnumerable<ApplicationRole> GetAllRoles ()
//		{
//			return _roleService.GetAllRoles ();
//		}

//		IEnumerable<ApplicationGroup> GetAllGroups ()
//		{
//			return _roleService.GetAllGroups ();
//		}

//		ApplicationGroup GetGroup (Guid groupId)
//		{
//			return GetAllGroups ().FirstOrDefault (g => g.Id == groupId);
//		}

//		///// <summary>
//		///// Checks to make sure that the current user belongs to one of the specified roles, otherwise a 403 Forbidden error is raised.
//		///// </summary>
//		///// <param name="roles">List of possible Roles the user can belong to.</param>
//		//protected void AuthorizeRoles (params string [] roles)
//		//{
//		//	bool hasAccess = false;
//		//	foreach (var role in roles) {
//		//		if (_roleService.DoesUserHaveRole (CurrentUser.UserName, role)) {
//		//			hasAccess = true;
//		//			break;
//		//		}
//		//	}
//		//	if (!hasAccess)
//		//		throw new HttpException ((int)HttpStatusCode.Forbidden, "Forbidden");
//		//}

//		///// <summary>
//		///// Checks to make sure the current user has the Admin role, otherwise raises a 403 Forbidden error.
//		///// </summary>
//		//protected void AuthorizeAdminOnly ()
//		//{
//		//	AuthorizeRoles ("Admin");
//		//}
//=======
//        ///// <summary>
//        ///// Checks to make sure the current user has the Admin role, otherwise raises a 403 Forbidden error.
//        ///// </summary>
//        //protected void AuthorizeAdminOnly ()
//        //{
//        //	AuthorizeRoles ("Admin");
//        //}
//>>>>>>> techcertain2019coreIdentity
//    }
//}
 
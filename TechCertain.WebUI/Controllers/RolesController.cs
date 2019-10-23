//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Net;
//using TechCertain.Domain.Entities;
//using TechCertain.Services.Interfaces;
//using TechCertain.WebUI.Models;
//using TechCertain.WebUI.Models.Permission;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using System.Threading.Tasks;

//namespace TechCertain.WebUI.Controllers
//{
    
//    public class RolesController : BaseController
//    {
//        RoleManager<IdentityRole> _roleManager;
//        UserManager<DealEngineUser> _userManager;
//        UserClaimsPrincipalFactory<DealEngineUser> _UserClaims;
//        IClaimService _ClaimService;
//        IUnitOfWork _unitOfWork;

//        public RolesController (IUserService userService,
//                  IUnitOfWork unitOfWork,
//                  UserManager<DealEngineUser> userManager,
//                  IClaimService claimService,
//                 RoleManager<IdentityRole> roleManager)
//		    : base (userService)    
//		{
//            _ClaimService = claimService;
//            _roleManager = roleManager;
//            _unitOfWork = unitOfWork;
//            _userManager = userManager;
//        }

//<<<<<<< HEAD
//		[HttpGet]
//		public async Task<IActionResult> Index ()
//		{
//			var models = new BaseListViewModel<RoleViewModel> ();
//			foreach (var role in _roleService.GetAllRoles()) {
//				models.Add (new RoleViewModel (role));
//			}
//=======
//        [HttpGet]
//        public IActionResult Index()
//        {
//            var models = new BaseListViewModel<ClaimViewModels>();
//            foreach (var claim in _ClaimService.GetAllClaims())
//            {
//                models.Add(new ClaimViewModels(claim));
//            }
//>>>>>>> techcertain2019coreIdentity

//            return View(models);
//        }

//<<<<<<< HEAD
//		public async Task<IActionResult> Details (int id)
//		{
//			return View ();
//		}

//		[HttpPost]
//		public async Task<IActionResult> Create (RoleViewModel roleViewModel)
//		{
//			try {
//                throw new Exception("Method will need to be re-written");
//                //if (!ModelState.IsValidField(nameof(roleViewModel.RoleName)))
//                //	return RedirectToAction ("Index");

//                ApplicationRole appRole = _roleService.CreateRole (roleViewModel.RoleName);
//				appRole.Description = roleViewModel.Description;
//				_roleService.UpdateRole (appRole);

//				return RedirectToAction ("Index");
//			}
//			catch {
//				return RedirectToAction ("Index");
//			}
//		}

//		[HttpPost]
//		public async Task<IActionResult> Edit (RoleViewModel roleViewModel)
//		{
//			try {
//				ApplicationRole appRole = _roleService.GetRole (roleViewModel.Id);
//				if (appRole == null)
//					throw new Exception ("Unable to find ApplicationRole with Id [" + roleViewModel.Id + "]");

//				appRole.Name = roleViewModel.RoleName;
//				appRole.Description = roleViewModel.Description;
//				_roleService.UpdateRole (appRole);

//				return Json (true);
//			}
//			catch (Exception ex) {				
//                throw new Exception("Method will need to be re-written " + ex.Message);
//				//return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, "Unable to save role with Id: [" + roleViewModel.Id + "]");
//			}
//		}

//		[HttpPost]
//		public async Task<IActionResult> Delete (Guid Id)
//		{
//			try {
//				if (!_roleService.DeleteRole (Id, CurrentUser))
//					throw new Exception ("Unable to delete ApplicationRole with Id [" + Id + "]");

//				return Json (true);
//			}
//			catch (Exception ex) {				
//                throw new Exception("Method will need to be re-written " + ex.Message);
//                //return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, ex.Message);
//			}
//		}
//=======

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


//            }
//            return View();
//        }


//        [HttpPost]
//        public ActionResult Create(ClaimViewModels claimViewModel)
//        {
//            using (IUnitOfWork uow = _unitOfWork.BeginUnitOfWork())
//            {
//                try
//                {
//                    //throw new Exception("Method will need to be re-written");
//                    //if (!ModelState.IsValidField(nameof(roleViewModel.RoleName)))
//                    //	return RedirectToAction ("Index");

//                    AuthClaims authclaims = _ClaimService.CreateClaims(CurrentUser, claimViewModel.ClaimName, claimViewModel.Description);
//                    uow.Commit();

//                    return RedirectToAction("Index");
//                }
//                catch
//                {
//                    return RedirectToAction("Index");
//                }

//            };
//        }

//        //[HttpPost]
//        //public ActionResult Edit (RoleViewModel roleViewModel)
//        //{
//        //	try {
//        //		ApplicationRole appRole = _roleService.GetRole (roleViewModel.Id);
//        //		if (appRole == null)
//        //			throw new Exception ("Unable to find ApplicationRole with Id [" + roleViewModel.Id + "]");

//        //		appRole.Name = roleViewModel.RoleName;
//        //		appRole.Description = roleViewModel.Description;
//        //		_roleService.UpdateRole (appRole);

//        //		return Json (true);
//        //	}
//        //	catch (Exception ex) {				
//        //              throw new Exception("Method will need to be re-written " + ex.Message);
//        //		//return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, "Unable to save role with Id: [" + roleViewModel.Id + "]");
//        //	}
//        //}

//        //[HttpPost]
//        //public ActionResult Delete (Guid Id)
//        //{
//        //	try {
//        //		if (!_roleService.DeleteRole (Id, CurrentUser))
//        //			throw new Exception ("Unable to delete ApplicationRole with Id [" + Id + "]");

//        //		return Json (true);
//        //	}
//        //	catch (Exception ex) {				
//        //              throw new Exception("Method will need to be re-written " + ex.Message);
//        //              //return new HttpStatusCodeResult (HttpStatusCode.InternalServerError, ex.Message);
//        //	}
//        //}
//>>>>>>> techcertain2019coreIdentity
//    }
//}

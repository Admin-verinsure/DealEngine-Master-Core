using Microsoft.AspNetCore.Mvc;
using DealEngine.Services.Interfaces;

namespace DealEngine.WebUI.Controllers
{
    public class ElmahController : BaseController
    {
        IAppSettingService _appSettingService;
        
        public ElmahController(
            IUserService userService,
            IAppSettingService appSettingService
            ) : base(userService)
        {
            _appSettingService = appSettingService;
        }

		[HttpGet]
        public IActionResult Index()
        {
            var superUserString = _appSettingService.GetSuperUser;
            var userList = superUserString.Split(',');

            return View();
        }
    }
}
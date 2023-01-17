using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pronia.Areas.Admin.Controllers
{
    public class DashboardController:Controller
    {

        [Area("Admin")]
        [Authorize("Moderator, Admin")]
        public IActionResult Index() 
        {
            return View();
        }
    }
}

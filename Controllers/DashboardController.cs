using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_authentication.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Eğer oturum yoksa login'e yönlendir
            if (User.Identity == null || !User.Identity.IsAuthenticated)
                return Redirect("/Account/Login");
            return View();
        }
    }
}

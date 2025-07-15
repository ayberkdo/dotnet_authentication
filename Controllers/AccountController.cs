using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using dotnet_authentication.Models;
using System.Threading.Tasks;

namespace dotnet_authentication.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Redirect("/Dashboard");
            // ReturnUrl parametresini kaldır
            if (Request.Query.ContainsKey("ReturnUrl"))
                return Redirect("/Account/Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            ViewData["email"] = email;
            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("email", "Email zorunludur.");
            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("password", "Şifre zorunludur.");
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View();
            }
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);
            if (result.Succeeded)
                return Redirect("/Dashboard");
            if (result.IsLockedOut)
                ModelState.AddModelError("", "Hesabınız kilitli.");
            else
                ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return Redirect("/Dashboard");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string email, string password)
        {
            ViewData["email"] = email;
            if (string.IsNullOrWhiteSpace(email))
                ModelState.AddModelError("email", "Email zorunludur.");
            if (string.IsNullOrWhiteSpace(password))
                ModelState.AddModelError("password", "Şifre zorunludur.");
            if (!ModelState.IsValid)
                return View();

            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                ModelState.AddModelError("email", "Bu email zaten kayıtlı.");
                return View();
            }
            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Redirect("/Dashboard");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/Account/Login");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;

namespace TSA.Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<LoginController> _logger;

        public LoginController(IUserService userService, ILogger<LoginController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult LogIn(LoginVM login, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //authenticate
                    User user = new User()
                    {
                        Email = login.Email,
                        Password = login.Password
                    };

                    if (_userService.SignIn(this.HttpContext, user, login.RememberMe).Result)
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home", null);
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrecto");

                }
                catch (System.Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "Ha ocurrido un error en el Sistema. Por favor contacte a su administrador.");
                    _logger.LogError(ex.Message);
                }
            }

            return View("Login");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOut(this.HttpContext);
            
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult AccessDenied(string returnUrl = "")
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("AccessDenied");
        }
    }
}

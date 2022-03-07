using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public UsersController(IUserService userService, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Policy = "Usuarios.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _userService.GetAllUsers());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Usuarios.Create")]
        public IActionResult Create()
        {
            UsersAndRolesVM usrRoles = new UsersAndRolesVM()
            {
                User = new User(),
                RolesList = _roleService.GetRolesList()
            };

            return View(usrRoles);
        }

        [HttpPost]
        [Authorize(Policy = "Usuarios.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsersAndRolesVM userAndRole)
        {
            if (ModelState.IsValid)
            {
                if (userAndRole.RoleId != 0)
                {
                    User user = await _userService.GetUserByEmail(userAndRole.User.Email);
                    if (user == null || user.IsActive == false)
                    {
                        ModelState.Clear();
                        try
                        {
                            await _userService.AddAndSave(this.HttpContext, userAndRole.User, userAndRole.RoleId);

                            TempData["message"] = "Created";
                        }
                        catch (System.Exception ex)
                        {
                            TempData["message"] = "Error";
                            ModelState.AddModelError(string.Empty, ex.Message);
                        }
                        userAndRole.User = new User();
                        userAndRole.RoleId = 0;
                        userAndRole.RolesList = _roleService.GetRolesList();
                        return View(userAndRole);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "El correo electrónico ingresado ya existe");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Por favor seleccione un Rol de Seguridad");
                }
            }
            userAndRole.RolesList = _roleService.GetRolesList();

            return View(userAndRole);
        }

        [HttpGet]
        [Authorize(Policy = "Usuarios.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            User user = await _userService.GetUserById(id);

            int roleId = await _userService.GetUserRoleByUserId(id);

            if (user == null || roleId == 0)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                UsersAndRolesVM usrRoles = new UsersAndRolesVM()
                {
                    User = user,
                    RoleId = roleId,
                    RolesList = _roleService.GetRolesList()
                };

                return View(usrRoles);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Usuarios.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsersAndRolesVM userAndRole)
        {
            if (ModelState.IsValid)
            {
                if (userAndRole.RoleId != 0)
                {
                    User user = await _userService.GetUserByEmail(userAndRole.User.Email);

                    if (user == null || userAndRole.User.Id == user.Id || userAndRole.User.IsActive == false)
                    {
                        try
                        {
                            await _userService.UpdateAndSave(this.HttpContext, userAndRole.User, userAndRole.RoleId);

                            TempData["message"] = "Edited";
                        }
                        catch (System.Exception ex)
                        {
                            TempData["message"] = "Error";
                            ModelState.AddModelError(string.Empty, ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "El correo electrónico ingresado ya existe");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Por favor seleccione un rol");
                }
            }
            userAndRole.RolesList = _roleService.GetRolesList();

            return View(userAndRole);
        }

        [HttpPost]
        [Authorize(Policy = "Usuarios.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userService.SoftDeleteAndSave(this.HttpContext, id);

                    TempData["message"] = "Deleted";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Policy = "Usuarios.Read")]
        public async Task<IActionResult> View(int id)
        {
            User user = await _userService.GetUserById(id);

            int roleId = await _userService.GetUserRoleByUserId(id);

            if (user == null || roleId == 0)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                UsersAndRolesVM usrRoles = new UsersAndRolesVM()
                {
                    User = user,
                    RoleId = roleId,
                    RolesList = _roleService.GetRolesList()
                };

                return View(usrRoles);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChangePassword(int id)
        {
            int userId = Convert.ToInt32(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

            if (userId == id)
            {
                User user = await _userService.GetUserById(id);

                UsersAndRolesVM usrRoles = new UsersAndRolesVM()
                {
                    User = user
                };

                return View(usrRoles);
            }
            else
            {
                return RedirectToAction("AccessDenied", "Login");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(UsersAndRolesVM userAndRole)
        {
            if (ModelState.IsValid)
            {
                int userId = Convert.ToInt32(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                if (userId == userAndRole.User.Id)
                {
                    User user = await _userService.GetUserById(userAndRole.User.Id);

                    bool passwordMatch = BCrypt.Net.BCrypt.Verify(userAndRole.User.ActualPassword, user.Password);

                    if (passwordMatch)
                    {
                        try
                        {
                            await _userService.ChangePassword(this.HttpContext, userAndRole.User);

                            TempData["message"] = "PasswordChanged";
                        }
                        catch (System.Exception ex)
                        {
                            TempData["message"] = "Error";
                            ModelState.AddModelError(string.Empty, ex.Message);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta");
                        return View();
                    }
                }
                else
                {
                    return RedirectToAction("AccessDenied", "Login");
                }
            }

            return RedirectToAction("Index","Home");
        }
    }
}

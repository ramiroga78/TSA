using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Entities;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;
using AutoMapper;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class ExternalUserController : Controller
    {
        private readonly IExternalUserService _userService;

        public ExternalUserController(IExternalUserService userService )
        {
            _userService = userService;
        }
        [HttpGet]
        [Authorize(Policy = "UsuariosExternos.Read")]
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
        [Authorize(Policy = "UsuariosExternos.Create")]
        public IActionResult Create()
        {
            return View(new ExternalUserDTO());
        }

        [HttpPost]
        [Authorize(Policy = "UsuariosExternos.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExternalUserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ExternalUser user = await _userService.GetUserByEmail(userDTO.Email);
                if (user == null || user.IsActive == false)
                {
                    ModelState.Clear();
                    try
                    {
                        await _userService.AddAndSave(this.HttpContext, userDTO);

                        TempData["message"] = "Created";
                    }
                    catch (System.Exception ex)
                    {
                        TempData["message"] = "Error";
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }

                    return View(userDTO);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "El correo electrónico ingresado ya existe");
                }
            }

            return View(userDTO);
        }

        [HttpGet]
        [Authorize(Policy = "UsuariosExternos.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetDTOUserById(id);

            if (user == null )
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(user);
            }
        }

        [HttpPost]
        [Authorize(Policy = "UsuariosExternos.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExternalUserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                ExternalUser user = await _userService.GetUserById(userDTO.Id);

                if (user != null )
                {
                    try
                    {
                        await _userService.UpdateAndSave(this.HttpContext, userDTO);
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
                    ModelState.AddModelError(string.Empty, "Usuario Externo no existe");
                }
            }
            return View(userDTO);
        }

        [HttpPost]
        [Authorize(Policy = "UsuariosExternos.Delete")]
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
        [Authorize(Policy = "UsuariosExternos.Read")]
        public async Task<IActionResult> View(int id)
        {
            ExternalUserDTO user = await _userService.GetDTOUserById(id);

            if (user == null )
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(user);
            }
        }

        //[HttpGet]
        //[Authorize]
        //public async Task<IActionResult> ChangePassword(int id)
        //{
        //    int userId = Convert.ToInt32(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

        //    if (userId == id)
        //    {
        //        User user = await _userService.GetUserById(id);

        //        UsersAndRolesVM usrRoles = new UsersAndRolesVM()
        //        {
        //            User = user
        //        };

        //        return View(usrRoles);
        //    }
        //    else
        //    {
        //        return RedirectToAction("AccessDenied", "Login");
        //    }
        //}

        //[HttpPost]
        //[Authorize]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ChangePassword(UsersAndRolesVM userAndRole)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        int userId = Convert.ToInt32(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

        //        if (userId == userAndRole.User.Id)
        //        {
        //            User user = await _userService.GetUserById(userAndRole.User.Id);

        //            bool passwordMatch = BCrypt.Net.BCrypt.Verify(userAndRole.User.ActualPassword, user.Password);

        //            if (passwordMatch)
        //            {
        //                try
        //                {
        //                    await _userService.ChangePassword(this.HttpContext, userAndRole.User);

        //                    TempData["message"] = "PasswordChanged";
        //                }
        //                catch (System.Exception ex)
        //                {
        //                    TempData["message"] = "Error";
        //                    ModelState.AddModelError(string.Empty, ex.Message);
        //                }
        //            }
        //            else
        //            {
        //                ModelState.AddModelError(string.Empty, "La contraseña actual es incorrecta");
        //                return View();
        //            }
        //        }
        //        else
        //        {
        //            return RedirectToAction("AccessDenied", "Login");
        //        }
        //    }

        //    return RedirectToAction("Index", "Home");
        //}
    }
}

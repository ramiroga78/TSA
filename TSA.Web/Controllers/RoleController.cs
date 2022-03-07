using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Services;


namespace TSA.Web.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize(Policy = "Roles.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _roleService.GetAllRoles());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Roles.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Roles.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleDTO roleDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.AddAndSave(this.HttpContext, roleDTO);
                    TempData["message"] = "Created";
                    return RedirectToAction("Create");
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(roleDTO);
        }

        [HttpGet]
        [Authorize(Policy = "Roles.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            RoleDTO roleDTO = await _roleService.GetRoleByIdAndModelToDto(id);

            if (roleDTO == null || roleDTO.IsActive == false)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(roleDTO);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Roles.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleDTO roleDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.UpdateAndSave(this.HttpContext, roleDTO);
                    TempData["message"] = "Edited";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(roleDTO);
        }

        [HttpPost]
        [Authorize(Policy = "Roles.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.SoftDeleteAndSave(this.HttpContext, id);
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
        [Authorize]
        public async Task<IActionResult> Functions(int id)
        {
            if (this.HttpContext.User.Claims
                .Any(c => (c.Type == "Roles.Read" || c.Type == "Roles.Update") && c.Value == "True"))
            {
                var roleFunctionList = _roleService.GetRoleFunctionsByIdAndModelToDto(id);
                ViewBag.Role = await _roleService.GetRoleByIdAndModelToDto(id);

                if (roleFunctionList == null || ViewBag.Role == null || ViewBag.Role.IsActive == false)
                {
                    TempData["message"] = "Error";

                    return RedirectToAction("Index");
                }
                else
                {
                    return View(roleFunctionList);
                }
            }
            else
            {
                return RedirectToAction("AccessDenied", "Login");
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Functions(List<RoleFunctionDTO> rolesFunctionsDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _roleService.ModifyRolesFunctionsAndSave(this.HttpContext, rolesFunctionsDTO);
                    TempData["message"] = "Edited";
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
        [Authorize(Policy = "Roles.Read")]
        public async Task<IActionResult> View(int id)
        {
            RoleDTO roleDTO = await _roleService.GetRoleByIdAndModelToDto(id);

            if (roleDTO == null || roleDTO.IsActive == false)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(roleDTO);
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class AlertController : Controller
    {
        private readonly IAlertService _alertService;
        public AlertController(IAlertService alertService)
        {
            _alertService = alertService;
        }

        [HttpGet]
        [Authorize(Policy = "Alertas.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _alertService.GetAllAlerts());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Alertas.Read")]
        public async Task<IActionResult> AlertUser(int id)
        {
            ViewBag.AlertUserId = id;
            ViewBag.Alert = await _alertService.GetAlertById(id);

            if (ViewBag.Alert == null || ViewBag.Alert.IsActive == false)
            {
                ModelState.Clear();
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(await _alertService.GetAllUsersByAlertIdAndModelToDto(id));
            }

        }

        [HttpGet]
        [Authorize(Policy = "Alertas.Read")]
        [Authorize(Policy = "Alertas.Create")]
        public async Task<IActionResult> CreateAlertUser(int id)
        {
            ViewBag.AlertId = id;
            ViewBag.AllUserByAlert = await _alertService.GetAllUsersAndModelToDto(id);
            ViewBag.Alert = await _alertService.GetAlertById(id);

            if (ViewBag.AllUserByAlert == null || ViewBag.Alert == null || ViewBag.Alert.IsActive == false)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(_alertService.CreateAlertUserDto());
            }

        }

        [HttpPost]
        [Authorize(Policy = "Alertas.Read")]
        [Authorize(Policy = "Alertas.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAlertUser(AlertUserDTO alertUserDto)
        {
            ViewBag.AlertId = alertUserDto.AlertId;

            if (ModelState.IsValid)
            {
                try
                {
                    await _alertService.CreateAlertUser(this.HttpContext, alertUserDto);

                    ViewBag.AllUserByAlert = await _alertService.GetAllUsersAndModelToDto(alertUserDto.AlertId);
                    ViewBag.Alert = await _alertService.GetAlertById(alertUserDto.AlertId);
                    TempData["message"] = "Created";

                    return View();
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewBag.AllUserByAlert = await _alertService.GetAllUsersAndModelToDto(alertUserDto.AlertId);
            ViewBag.Alert = await _alertService.GetAlertById(alertUserDto.AlertId);

            return View(alertUserDto);
        }

        [HttpGet]
        [Authorize(Policy = "Alertas.Read")]
        [Authorize(Policy = "Alertas.Update")]
        public async Task<IActionResult> EditAlertUser(int id)
        {
            try
            {
                var alertUserDto = await _alertService.GetUserAndModelToAlertUserDtoById(id);

                if (alertUserDto != null)
                {
                    ViewBag.AllUserByAlert = await _alertService.GetAllUsersAndModelToDto(alertUserDto.AlertId);
                    ViewBag.AlertId = alertUserDto.AlertId;
                    ViewBag.Alert = await _alertService.GetAlertById(alertUserDto.AlertId);

                    return View(alertUserDto);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            TempData["message"] = "Error";

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = "Alertas.Read")]
        [Authorize(Policy = "Alertas.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAlertUser(AlertUserDTO alertUserDto)
        {
            ViewBag.AlertId = alertUserDto.AlertId;

            if (ModelState.IsValid)
            {
                try
                {
                    await _alertService.UpdateAndSave(this.HttpContext, alertUserDto);
                    TempData["message"] = "Edited";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            ViewBag.AllUserByAlert = await _alertService.GetAllUsersAndModelToDto(alertUserDto.AlertId);
            ViewBag.Alert = await _alertService.GetAlertById(alertUserDto.AlertId);

            return RedirectToAction("AlertUser", new { Id = alertUserDto.AlertId });
        }

        [HttpPost]
        [Authorize(Policy = "Alertas.Read")]
        [Authorize(Policy = "Alertas.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAlertUser(int id)
        {
            var alertUserDto = await _alertService.GetUserAndModelToAlertUserDtoById(id);

            if (ModelState.IsValid)
            {
                try
                {
                    await _alertService.DeleteAlertUser(this.HttpContext, id);
                    TempData["message"] = "Deleted";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return RedirectToAction("AlertUser", new { Id = alertUserDto.AlertId });
        }

    }
}

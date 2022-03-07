using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Repositories;
using TSALibrary.Models;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class NTPServersController : Controller
    {
        private readonly INTPServerService _nTPServerService;
        public NTPServersController(INTPServerService nTPServerService)
        {
            _nTPServerService = nTPServerService;
        }

        [HttpGet]
        [Authorize(Policy = "Servidores NTP.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _nTPServerService.GetAllNTPServers());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Servidores NTP.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Servidores NTP.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NTPServerDTO nTPServerDTO)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                try
                {
                    await _nTPServerService.AddAndSave(this.HttpContext, nTPServerDTO);
                    TempData["message"] = "Created";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View();
        }

        //[HttpGet]
        //[Authorize(Policy = "Servidores NTP.Update")]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    try
        //    {
        //        NTPServerDTO ntpServer = await _nTPServerService.GetNTPServerById(id);

        //        if (ntpServer != null && ntpServer.IsActive == true)
        //        {
        //            return View(ntpServer);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        ModelState.AddModelError(string.Empty, ex.Message);
        //    }

        //    TempData["message"] = "Error";

        //    return RedirectToAction("Index");
        //}

        //[HttpPost]
        //[Authorize(Policy = "Servidores NTP.Update")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(NTPServerDTO nTPServerDTO)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            await _nTPServerService.UpdateAndSave(this.HttpContext, nTPServerDTO);
        //            TempData["message"] = "Edited";
        //        }
        //        catch (System.Exception ex)
        //        {
        //            TempData["message"] = "Error";
        //            ModelState.AddModelError(string.Empty, ex.Message);
        //        }
        //    }
        //    return View(nTPServerDTO);
        //}

        [HttpPost]
        [Authorize(Policy = "Servidores NTP.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _nTPServerService.SoftDeleteAndSave(this.HttpContext, id);
                    TempData["message"] = "Deleted";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Policy = "Servidores NTP.Read")]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                NTPServerDTO ntpServer = await _nTPServerService.GetNTPServerById(id);

                if (ntpServer != null && ntpServer.IsActive == true)
                {
                    return View(ntpServer);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            TempData["message"] = "Error";

            return RedirectToAction("Index");
        }
    }
}

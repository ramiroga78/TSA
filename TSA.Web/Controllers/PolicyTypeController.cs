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
    public class PolicyTypeController : Controller
    {
        private readonly IPolicyTypeService _policyTypeService;
        public PolicyTypeController(IPolicyTypeService policyTypeService)
        {
            _policyTypeService = policyTypeService;
        }

        [HttpGet]
        [Authorize(Policy = "Policies.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _policyTypeService.GetAllPoliciesTypes());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Policies.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Policies.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PolicyTypeDTO policyTypeDto)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                try
                {
                    await _policyTypeService.AddAndSave(this.HttpContext, policyTypeDto);
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

        [HttpGet]
        [Authorize(Policy = "Policies.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                PolicyTypeDTO policy = await _policyTypeService.GetPolicyTypeByIdAndModelToDto(id);

                if (policy != null && policy.IsActive == true)
                {
                    return View(policy);
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
        [Authorize(Policy = "Policies.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PolicyTypeDTO policyTypeDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _policyTypeService.UpdateAndSave(this.HttpContext, policyTypeDto);
                    TempData["message"] = "Edited";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(policyTypeDto);
        }

        [HttpPost]
        [Authorize(Policy = "Policies.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _policyTypeService.SoftDeleteAndSave(this.HttpContext, id);
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
        [Authorize(Policy = "Policies.Read")]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                PolicyTypeDTO policy = await _policyTypeService.GetPolicyTypeByIdAndModelToDto(id);

                if (policy != null && policy.IsActive == true)
                {
                    return View(policy);
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

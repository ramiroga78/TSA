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
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        //TYPES
        [HttpGet]
        [Authorize(Policy = "Profiles.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _profileService.GetAllProfileTypes());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Profiles.Read")]
        public async Task<IActionResult> View(int id)
        {
            ProfileTypeDTO profileTypeDTO = await _profileService.GetProfileTypeByIdAndModelToDto(id);

            if (profileTypeDTO == null || profileTypeDTO.IsActive == false)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                return View(profileTypeDTO);
            }
        }

        [HttpGet]
        [Authorize(Policy = "Profiles.Create")]
        public IActionResult Create()
        {
            //var profileTypeDTO = _profileService.CreateProfileTypeDTO();
            return View();// profileTypeDTO);
        }

        [HttpPost]
        [Authorize(Policy = "Profiles.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfileTypeDTO profileTypeDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _profileService.AddAndSaveType(this.HttpContext, profileTypeDTO);
                    TempData["message"] = "Created";
                    return RedirectToAction("Create");
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(profileTypeDTO);
        }

        [HttpGet]
        [Authorize(Policy = "Profiles.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            ProfileTypeDTO profileTypeDTO = await _profileService.GetProfileTypeByIdAndModelToDto(id);

            if (profileTypeDTO == null || profileTypeDTO.IsActive == false)
            {
                TempData["message"] = "Error";
                return RedirectToAction("Index");
            }
            else
            {
                return View(profileTypeDTO);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Profiles.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileTypeDTO profileTypeDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _profileService.UpdateAndSaveType(this.HttpContext, profileTypeDTO);
                    TempData["message"] = "Edited";
                    return View(profileTypeDTO);
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(profileTypeDTO);
        }

        [HttpPost]
        [Authorize(Policy = "Profiles.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSaveType(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _profileService.SoftDeleteAndSaveType(this.HttpContext, id);
                    TempData["message"] = "Deleted";
                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return RedirectToAction("Index");
        }

        //VALUES
        [HttpGet]
        [Authorize(Policy = "Profiles.Read")]
        public async Task<IActionResult> IndexProfileValues(int id)
        {
            try
            {
                var profileValues = await _profileService.GetProfileValuesByTypeId(id);
                ViewBag.ProfileType = await _profileService.GetProfileTypeByIdAndModelToDto(id);
                return View(profileValues);
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> ViewProfileValues(int id)
        {
            ProfileValueDTO profileValueDTO = await _profileService.GetProfileValueByIdAndModelToDto(id);

            if (profileValueDTO == null || profileValueDTO.IsActive == false)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ProfileType = await _profileService.GetProfileTypeByIdAndModelToDto(profileValueDTO.ProfileTypeId);
                return View(profileValueDTO);
            }
        }

        [HttpGet]
        [Authorize(Policy = "Profiles.Create")]
        public async Task<IActionResult> CreateProfileValue(int Id)
        {
            var profileValueDTO = _profileService.CreateProfileValueDTO();
            ViewBag.ProfileType = await _profileService.GetProfileTypeByIdAndModelToDto(Id);
            return View(profileValueDTO);
        }

        [HttpPost]
        [Authorize(Policy = "Profiles.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProfileValue(ProfileValueDTO profileValueDTO, int Id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ViewBag.ProfileType = await _profileService.GetProfileTypeByIdAndModelToDto(Id);
                    profileValueDTO.ProfileTypeId = Id;
                    await _profileService.AddAndSaveValue(this.HttpContext, profileValueDTO);
                    TempData["message"] = "Created";
                    return RedirectToAction("Create");
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(profileValueDTO);
        }

        [HttpGet]
        [Authorize(Policy = "Profiles.Update")]
        public async Task<IActionResult> EditProfileValue(int id)
        {
            ProfileValueDTO profileValueDTO = await _profileService.GetProfileValueByIdAndModelToDto(id);

            if (profileValueDTO == null || profileValueDTO.IsActive == false)
            {
                TempData["message"] = "Error";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ProfileType = await _profileService.GetProfileTypeByIdAndModelToDto(profileValueDTO.ProfileTypeId);
                return View(profileValueDTO);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Profiles.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfileValue(ProfileValueDTO profileValueDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _profileService.UpdateAndSaveValue(this.HttpContext, profileValueDTO);
                    TempData["message"] = "Edited";
                    return View(profileValueDTO);
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(profileValueDTO);
        }

        [HttpPost]
        [Authorize(Policy = "Profiles.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSaveValue(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _profileService.SoftDeleteAndSaveValue(this.HttpContext, id);
                    TempData["mwssege"] = "Delete";
                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return RedirectToAction("Index");
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Repositories;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class CertificateOrganizationController : Controller
    {
        private readonly ICertificateOrganizationService _certificateOrganizationService;
        public CertificateOrganizationController(ICertificateOrganizationService certificateOrganizationService)
        {
            _certificateOrganizationService = certificateOrganizationService;
        }

        [HttpGet]
        [Authorize]
        public async Task<JsonResult> LoadOrgDetails(int id)
        {
            CertificateOrganization org = await _certificateOrganizationService.GetCertificateOrganizationById(id);

            return Json(org);
        }

        [HttpGet]
        [Authorize(Policy = "Organizaciones.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _certificateOrganizationService.GetAllCertificateOrganization());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Organizaciones.Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var certificateOrganizationDTO = _certificateOrganizationService.CreateCertificateOrganizationDto();
                certificateOrganizationDTO.CertificateOrganizationTypes = (List<CertificateOrganizationType>)await _certificateOrganizationService.GetAllCertificateOrganizationTypes();
                certificateOrganizationDTO.CountryList = _certificateOrganizationService.GetCountryList();

                return View(certificateOrganizationDTO);
            }
            catch (System.Exception)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize(Policy = "Organizaciones.Create")]
        public async Task<IActionResult> Create(CertificateOrganizationDTO certificateOrganizationDTO)
        {
            try
            {
                certificateOrganizationDTO.CertificateOrganizationType = await _certificateOrganizationService.
                                            GetCertificateOrganizationTypeById(certificateOrganizationDTO.CertificateOrganizationTypeId);
                if (ModelState.IsValid)
                {
                    ModelState.Clear();
                    try
                    {
                        await _certificateOrganizationService.AddAndSave(this.HttpContext, certificateOrganizationDTO);
                        TempData["message"] = "Created";
                    }
                    catch (System.Exception ex)
                    {
                        TempData["message"] = "Error";
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }
                //SE ASIGNAN DATOS NUEVAMENTE PARA QUE LA VISTA PUEDA MOSTRARSE
                CertificateOrganizationDTO cerOrganizationDTO = new CertificateOrganizationDTO();
                cerOrganizationDTO.CertificateOrganizationTypes = (List<CertificateOrganizationType>)await _certificateOrganizationService.GetAllCertificateOrganizationTypes();
                cerOrganizationDTO.CountryList = _certificateOrganizationService.GetCountryList();

                return View(cerOrganizationDTO);
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Policy = "Organizaciones.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var certificateOrganizationDto = await _certificateOrganizationService.GetCertificateOrganizationByIdAndModelToDto(id);

                if (certificateOrganizationDto != null && certificateOrganizationDto.IsActive == true)
                {
                    certificateOrganizationDto.CertificateOrganizationTypes = (List<CertificateOrganizationType>)await _certificateOrganizationService.GetAllCertificateOrganizationTypes();

                    return View(certificateOrganizationDto);
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
        [Authorize(Policy = "Organizaciones.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CertificateOrganizationDTO certificateOrganizationDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _certificateOrganizationService.UpdateAndSave(this.HttpContext, certificateOrganizationDTO);
                    TempData["message"] = "Edited";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            //SE ASIGNAN DATOS NUEVAMENTE PARA QUE LA VISTA PUEDA MOSTRARSE
            var certificateOrganizationDto = await _certificateOrganizationService.GetCertificateOrganizationByIdAndModelToDto(certificateOrganizationDTO.Id);
            certificateOrganizationDto.CertificateOrganizationTypes = (List<CertificateOrganizationType>)await _certificateOrganizationService.GetAllCertificateOrganizationTypes();

            return View(certificateOrganizationDto);
        }

        [HttpPost]
        [Authorize(Policy = "Organizaciones.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _certificateOrganizationService.SoftDeleteAndSave(this.HttpContext, id);
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
        [Authorize(Policy = "Organizaciones.Read")]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                var certificateOrganizationDto = await _certificateOrganizationService.GetCertificateOrganizationByIdAndModelToDto(id);

                if (certificateOrganizationDto != null && certificateOrganizationDto.IsActive == true)
                {
                    certificateOrganizationDto.CertificateOrganizationTypes = (List<CertificateOrganizationType>)await _certificateOrganizationService.GetAllCertificateOrganizationTypes();

                    return View(certificateOrganizationDto);
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

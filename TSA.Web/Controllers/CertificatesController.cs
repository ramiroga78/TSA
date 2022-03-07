using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Helpers;
//using System.Web.Mvc;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;
using TSA.Infrastructure.Repositories;
using TSA.Infrastructure.ViewModels;
using TSALibrary.Models;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class CertificatesController : Controller
    {
        private readonly ICertificateService _certificatesService;
        private readonly IPolicyTypeService _policyTypeService;
        private readonly IProfileService _profileService;
        private readonly ICertificateOrganizationService _certificateOrganizationService;

        private CertificatesVM CreateCertificateVM()
        {
            CertificatesVM certificate = new CertificatesVM()
            {
                Certificate = new Certificate(),
                CommonName = "",
                IssuersList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Issuer"),
                SubjectsList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Subject"),
                ProfilesTypesList = _profileService.GetProfilesTypeList(),
                ProfilesValuesList = _profileService.GetProfilesValuesList(),
                PoliciesTypesList = _policyTypeService.GetPoliciesTypesList()
            };

            return certificate;
        }

        public CertificatesController(ICertificateService certificatesService, ICertificateOrganizationService certificateOrganizationService, IPolicyTypeService policyTypeService, IProfileService profileService)
        {
            _certificatesService = certificatesService;
            _policyTypeService = policyTypeService;
            _profileService = profileService;
            _certificateOrganizationService = certificateOrganizationService;
        }

        [HttpGet]
        [Authorize]
        private IList<ProfileValue> GetProfilesValues(int profileTypeId)
        {
            return _profileService.GetProfilesValuesByTypeList(profileTypeId);
        }

        [HttpGet]
        [Authorize]
        public JsonResult LoadProfilesValues(int profileTypeId)
        {
            IList<ProfileValue> profilesValues = GetProfilesValues(profileTypeId);

            return Json(profilesValues);
        }

        [HttpGet]
        [Authorize(Policy = "Certificados.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _certificatesService.GetAllCertificatesVM());
            }
            catch (System.Exception)
            {
                TempData["message"] = "Error";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Certificados.Create")]
        public IActionResult Create()
        {
            try
            {
                CertificatesVM certificate = CreateCertificateVM();

                ViewBag.ListOfProfilesTypes = certificate.ProfilesTypesList;

                return View(certificate);
            }
            catch (System.Exception)
            {
                TempData["message"] = "Error";

                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [Authorize(Policy = "Certificados.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificatesVM certificateVM)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                try
                {
                    int count = Request.Form.Count;

                    //IList<PoliciesType> policies = new List<PoliciesType>();
                    ICollection<CertificatePolicy> policies = new Collection<CertificatePolicy>();
                    ICollection<CertificateProfile> profiles = new Collection<CertificateProfile>();

                    DateTime addDate = DateTime.Now;
                    int addUser = Convert.ToInt32(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                    for (int i = 0; i <= count; i++)
                    {
                        var policy = Request.Form["Policy[" + i + "]"];
                        var policyvalue = Request.Form["Policyvalue[" + i + "]"];
                        var profile = Request.Form["Profile[" + i + "]"];
                        var profilevalue = Request.Form["ProfileValue[" + i + "]"];

                        if ((policy.ToString() != "") && (policyvalue.ToString() != ""))
                        {
                            policies.Add(new CertificatePolicy
                            {
                                AddDate = addDate,
                                AddUserId = addUser,
                                IdPolicyType = Convert.ToInt32(policy),
                                PolicyValue = policyvalue
                            });
                        }

                        if ((profile.ToString() != "") && (profilevalue.ToString() != ""))
                        {
                            profiles.Add(new CertificateProfile
                            {
                                AddDate = addDate,
                                AddUserId = addUser,
                                ProfileTypeId = Convert.ToInt32(profile),
                                IdProfileValue = Convert.ToInt32(profilevalue)
                            });
                        }
                    }

                    certificateVM.Certificate.CertificatePolicies = policies;
                    certificateVM.Certificate.CertificateProfiles = profiles;

                    DateTime fromdate = DateTime.ParseExact(Request.Form["fromdate"], "MM/dd/yyyy", null);
                    DateTime todate = DateTime.ParseExact(Request.Form["todate"], "MM/dd/yyyy", null);

                    certificateVM.Certificate.AddDate = addDate;
                    certificateVM.Certificate.AddUserId = addUser;
                    certificateVM.Certificate.NotBefore = fromdate;
                    certificateVM.Certificate.NotAfter = todate;

                    await _certificatesService.AddAndSave(this.HttpContext, certificateVM);

                    TempData["message"] = "Created";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            CertificatesVM certVM = CreateCertificateVM();

            return View(certVM);
        }

        [HttpGet]
        [Authorize(Policy = "Certificados.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Certificate cert = await _certificatesService.GetCertificateByIdIncludingProfilesAndPolicies(id);

                if (cert != null && cert.IsActive == true)
                {
                    CertificatesVM certificate = new CertificatesVM()
                    {
                        Certificate = cert,
                        CommonName = "",
                        IssuersList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Issuer"),
                        SubjectsList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Subject"),
                        ProfilesTypesList = _profileService.GetProfilesTypeList(),
                        ProfilesValuesList = _profileService.GetProfilesValuesList(),
                        PoliciesTypesList = _policyTypeService.GetPoliciesTypesList()
                    };
                    return View(certificate);
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
        [Authorize(Policy = "Certificados.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CertificatesVM certVM)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                try
                {
                    int count = Request.Form.Count;

                    DateTime addDate = DateTime.Now;
                    int addUser = Convert.ToInt32(this.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);

                    ICollection<CertificatePolicy> policies = new Collection<CertificatePolicy>();
                    ICollection<CertificateProfile> profiles = new Collection<CertificateProfile>();

                    for (int i = 0; i <= count; i++)
                    {
                        var policy = Request.Form["Policy[" + i + "]"];
                        var policyvalue = Request.Form["Policyvalue[" + i + "]"];
                        var profile = Request.Form["Profile[" + i + "]"];
                        var profilevalue = Request.Form["ProfileValue[" + i + "]"];

                        if ((policy.ToString() != "") && (policyvalue.ToString() != ""))
                        {
                            policies.Add(new CertificatePolicy
                            {
                                CertificateId = certVM.Certificate.Id,
                                AddDate = addDate,
                                AddUserId = addUser,
                                IdPolicyType = Convert.ToInt32(policy),
                                PolicyValue = policyvalue
                            });
                        }

                        if ((profile.ToString() != "") && (profilevalue.ToString() != ""))
                        {
                            profiles.Add(new CertificateProfile
                            {
                                CertificateId = certVM.Certificate.Id,
                                AddDate = addDate,
                                AddUserId = addUser,
                                ProfileTypeId = Convert.ToInt32(profile),
                                IdProfileValue = Convert.ToInt32(profilevalue)
                            });
                        }
                    }

                    DateTime fromdate = DateTime.ParseExact(Request.Form["fromdate"], "MM/dd/yyyy", null);
                    DateTime todate = DateTime.ParseExact(Request.Form["todate"], "MM/dd/yyyy", null);

                    certVM.Certificate.NotBefore = fromdate;
                    certVM.Certificate.NotAfter = todate;

                    await _certificatesService.UpdateAndSave(this.HttpContext, certVM, policies, profiles);
                    TempData["message"] = "Edited";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            certVM.Certificate = await _certificatesService.GetCertificateByIdIncludingProfilesAndPolicies(certVM.Certificate.Id);
            certVM.IssuersList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Issuer");
            certVM.SubjectsList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Subject");
            certVM.ProfilesTypesList = _profileService.GetProfilesTypeList();
            certVM.ProfilesValuesList = _profileService.GetProfilesValuesList();
            certVM.PoliciesTypesList = _policyTypeService.GetPoliciesTypesList();

            return View(certVM);
        }

        [HttpPost]
        [Authorize(Policy = "Certificados.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _certificatesService.SoftDeleteAndSave(this.HttpContext, id);
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
        [Authorize(Policy = "Certificados.Read")]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                Certificate cert = await _certificatesService.GetCertificateByIdIncludingProfilesAndPolicies(id);

                if (cert != null && cert.IsActive == true)
                {
                    CertificatesVM certificate = new CertificatesVM()
                    {
                        Certificate = cert,
                        CommonName = "",
                        IssuersList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Issuer"),
                        SubjectsList = _certificateOrganizationService.GetAllCertificateOrganizationByType("Subject"),
                        ProfilesTypesList = _profileService.GetProfilesTypeList(),
                        ProfilesValuesList = _profileService.GetProfilesValuesList(),
                        PoliciesTypesList = _policyTypeService.GetPoliciesTypesList()
                    };
                    return View(certificate);
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

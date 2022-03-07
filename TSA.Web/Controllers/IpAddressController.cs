using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;

namespace TSA.Web.Controllers
{
    [Authorize]
    public class IpAddressController : Controller
    {
        private readonly IIpAddresService _ipAddresService;

        public IpAddressController(IIpAddresService ipAddressService, IUnitOfWork unitOfWork)
        {
            _ipAddresService = ipAddressService;
        }

        [HttpGet]
        [Authorize(Policy = "Permisos IP.Read")]
        public async Task<IActionResult> Index()
        {
            try
            {
                return View(await _ipAddresService.GetAllIpAddresses());
            }
            catch (System.Exception ex)
            {
                TempData["message"] = "Error";
                ModelState.AddModelError(string.Empty, ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        [Authorize(Policy = "Permisos IP.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Permisos IP.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IpAddressDTO ipAddressDTO)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();

                try
                {
                    await _ipAddresService.AddAndSave(this.HttpContext, ipAddressDTO);
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
        [Authorize(Policy = "Permisos IP.Update")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                IpAddressDTO ipAddressDTO = await _ipAddresService.GetAllIpAddressByIdAndModelToDto(id);

                if (ipAddressDTO != null && ipAddressDTO.IsActive == true)
                {
                    return View(ipAddressDTO);
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
        [Authorize(Policy = "Permisos IP.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IpAddressDTO ipAddressDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ipAddresService.UpdateAndSave(this.HttpContext, ipAddressDTO);
                    TempData["message"] = "Edited";
                }
                catch (System.Exception ex)
                {
                    TempData["message"] = "Error";
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View();
        }

        [HttpPost]
        [Authorize(Policy = "Permisos IP.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDeleteAndSave(int id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _ipAddresService.SoftDeleteAndSave(this.HttpContext, id);
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
        [Authorize(Policy = "Permisos IP.Read")]
        public async Task<IActionResult> View(int id)
        {
            try
            {
                IpAddressDTO ipAddressDTO = await _ipAddresService.GetAllIpAddressByIdAndModelToDto(id);

                if (ipAddressDTO != null && ipAddressDTO.IsActive == true)
                {
                    return View(ipAddressDTO);
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

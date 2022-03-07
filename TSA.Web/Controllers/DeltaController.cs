using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSA.Infrastructure.DTOs;
using TSA.Infrastructure.Interfaces;

namespace TSA.Web.Controllers
{
    //[Authorize]
    public class DeltaController : Controller
    {
        private readonly IDeltaService _deltaService;

        public DeltaController(IDeltaService deltaService)
        {
            _deltaService = deltaService;
        }

        [HttpGet]
        [Authorize(Policy = "Deltas.Read")]
        public async Task<IActionResult> Index()
        {
            return View(await _deltaService.GetAllDeltas());
        }

        //DELTA
        [HttpGet]
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Update")]
        public async Task<IActionResult> DeltaSetting(int id)
        {
           try
           {
               var deltaDTO = await _deltaService.GetAllDeltaByIdAndModelToDto(id);
          
            if (deltaDTO != null)
            {
                return View(deltaDTO);
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
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Update")]
        public async Task <IActionResult> DeltaSetting(DeltaDTO deltaDTO)
        {
            if(ModelState.IsValid)
            {
                await _deltaService.UpdateAndSave(this.HttpContext, deltaDTO);
                //revisar
                deltaDTO = await _deltaService.GetAllDeltaByIdAndModelToDto(deltaDTO.Id);
                TempData["message"] = "Edited";
                return View(deltaDTO);
            }
            return View(deltaDTO);
        }

        //DELTA USER
        [HttpGet]
        [Authorize(Policy = "Deltas.Read")]
        public async Task<IActionResult> DeltaUser(int Id)
        {
            ViewBag.DeltaUserId = Id;
            return View(await _deltaService.GetAllUsersByDeltaIdAndModelToDto(Id));
        }

        [HttpGet]
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Create")]
        public async Task<IActionResult> CreateDeltaUser(int Id)
        {
            ViewBag.DeltaId = Id;
            ViewBag.AllUserByDelta = await _deltaService.GetAllUsersAndModelToDto(Id);
            return View(_deltaService.CreateDeltaUserDTO());
        }

        [HttpPost]
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeltaUser(DeltaUserDTO deltaUserDTO)
        {
            ViewBag.DeltaId = deltaUserDTO.Id;
            if (ModelState.IsValid)
            {
                await _deltaService.CreateDeltaUser(this.HttpContext, deltaUserDTO);
                ViewBag.AllUserByDelta = await _deltaService.GetAllUsersByDeltaIdAndModelToDto(deltaUserDTO.DeltaId);
                TempData["message"] = "Created";

                return View();
            }
            ViewBag.AllUSerByDelta = await _deltaService.GetAllUsersByDeltaIdAndModelToDto(deltaUserDTO.DeltaId);
            return View(deltaUserDTO);
        }

        [HttpGet]
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Update")]
        public async Task<IActionResult> EditDeltaUser(int Id)
        {
            try
            {
                var deltaUserDTO = await _deltaService.GetUserAndModelToDeltaUserDtoById(Id);
                if (deltaUserDTO != null)
                {
                    ViewBag.AllUserByDelta = await _deltaService.GetAllUsersAndModelToDto(deltaUserDTO.DeltaId);
                    ViewBag.DeltaId = deltaUserDTO.DeltaId;
                    ViewBag.Delta = await _deltaService.GetAllDeltaByIdAndModelToDto(deltaUserDTO.DeltaId);


                    return View(deltaUserDTO);
                }
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Update")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeltaUser(DeltaUserDTO deltaUserDTO)
        {
            ViewBag.DeltaId = deltaUserDTO.Id;
            if (ModelState.IsValid)
            {
                await _deltaService.UpdateAndSaveDeltaUser(this.HttpContext, deltaUserDTO);
                ViewBag.AllUserByDelta = await _deltaService.GetAllUsersByDeltaIdAndModelToDto(deltaUserDTO.DeltaId);
                TempData["message"] = "Edited";
                return RedirectToAction("DeltaUser", new { Id = deltaUserDTO.DeltaId });
            }
            ViewBag.AllUserByDelta = await _deltaService.GetAllUsersByDeltaIdAndModelToDto(deltaUserDTO.DeltaId);
            return RedirectToAction("DeltaUser", new { Id = deltaUserDTO.Id });
        }

        [HttpPost]
        [Authorize(Policy = "Deltas.Read")]
        [Authorize(Policy = "Deltas.Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDeltaUser(int Id)
        {
            var deltaUserDTO = await _deltaService.GetUserAndModelToDeltaUserDtoById(Id);
            if (ModelState.IsValid)
            {
                await _deltaService.DeleteDeltaUser(this.HttpContext, Id);
                TempData["message"] = "Deleted";
                return RedirectToAction("DeltaUser", new { Id = deltaUserDTO.DeltaId });
            }
            return RedirectToAction("DeltaUser", new { Id = deltaUserDTO.DeltaId });
        }
    }
}

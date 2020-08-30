using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class NationalParksController : Controller
    {
        private readonly INationalParkRepository nationalParkRepository;

        public NationalParksController(INationalParkRepository nationalParkRepository)
        {
            this.nationalParkRepository = nationalParkRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllNationalPark()
        {
            return Json(new { data = await nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken")) });
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Upsert(int? id)
        {
            var nationalPark = new NationalPark();

            if(id == null){
                return View(nationalPark);
            }

            nationalPark = await nationalParkRepository.GetAsync(SD.NationalParkAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWTToken"));

            if(nationalPark== null)
            {
                return NotFound();
            }

            return View(nationalPark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(NationalPark nationalPark)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] picture = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using(var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            picture = ms1.ToArray();
                        }
                    }
                    nationalPark.Picture = picture;
                }
                else
                {
                    var objFromDb = await nationalParkRepository.GetAsync(SD.NationalParkAPIPath, nationalPark.Id, HttpContext.Session.GetString("JWTToken"));
                    nationalPark.Picture = objFromDb.Picture;
                }

                if(nationalPark.Id == 0)
                {
                    await nationalParkRepository.CreateAsync(SD.NationalParkAPIPath, nationalPark, HttpContext.Session.GetString("JWTToken"));
                }
                else
                {
                    await nationalParkRepository.UpdateAsync(SD.NationalParkAPIPath + nationalPark.Id, nationalPark, HttpContext.Session.GetString("JWTToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return View(nationalPark);
            }
        }

        [Authorize("Admin")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await nationalParkRepository.DeleteAsync(SD.NationalParkAPIPath, id, HttpContext.Session.GetString("JWTToken"));
            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }
            return Json(new { success = false, message = "Delete not successful" });
        }

    }
}

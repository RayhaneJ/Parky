using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyWeb.Models;
using ParkyWeb.Repository.IRepository;
using ParkyWeb.ViewModel;

namespace ParkyWeb.Controllers
{
    [Authorize]
    public class TrailsController : Controller
    {
        private readonly INationalParkRepository nationalParkRepository;
        private readonly ITrailRepository trailRepository;

        public TrailsController(INationalParkRepository nationalParkRepository, ITrailRepository trailRepository)
        {
            this.nationalParkRepository = nationalParkRepository;
            this.trailRepository = trailRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

       
        public async Task<IActionResult> Upsert(int? id)
        {
            IEnumerable<NationalPark> npList = await nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken"));

            var trailsVM = new TrailViewModel()
            {
                NationalParkList = npList.Select(n => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = n.Name,
                    Value = n.Id.ToString()
                }),
                Trail = new Trail()
            };

            if(id == null)
            {
                return View(trailsVM);
            }

            trailsVM.Trail = await trailRepository.GetAsync(SD.TrailAPIPath, id.GetValueOrDefault(), HttpContext.Session.GetString("JWTToken"));

            if(trailsVM.Trail == null)
            {
                return NotFound();
            }

            return View(trailsVM );
        }

        [HttpPost]
        [Authorize("Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TrailViewModel trailVM)
        {
            if (ModelState.IsValid)
            {
                if(trailVM.Trail.Id == 0)
                {
                    await trailRepository.CreateAsync(SD.TrailAPIPath, trailVM.Trail, HttpContext.Session.GetString("JWTToken"));
                }
                else
                {
                    await trailRepository.UpdateAsync(SD.TrailAPIPath + trailVM.Trail.Id, trailVM.Trail, HttpContext.Session.GetString("JWTToken"));
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                IEnumerable<NationalPark> npList = await nationalParkRepository.GetAllAsync(SD.NationalParkAPIPath, HttpContext.Session.GetString("JWTToken"));

                var trailsVM = new TrailViewModel()
                {
                    NationalParkList = npList.Select(n => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Text = n.Name,
                        Value = n.Id.ToString()
                    }),
                    Trail = trailVM.Trail
                };

                return View(trailsVM);
            }
        }

        public async Task<IActionResult> GetAllTrails()
        {
            return Json(new { data = await trailRepository.GetAllAsync(SD.TrailAPIPath, HttpContext.Session.GetString("JWTToken")) });
        }

        [Authorize("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var status = await trailRepository.DeleteAsync(SD.TrailAPIPath, id, HttpContext.Session.GetString("JWTToken"));
            if (status)
            {
                return Json(new { success = true, message = "Delete successful" });
            }
            return Json(new { success = false, message = "Delete not successful" });
        }
    }
}

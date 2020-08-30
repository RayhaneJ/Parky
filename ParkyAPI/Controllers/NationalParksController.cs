using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    /*[Route("api/[controller]")]*/
    [Authorize]
    [Route("api/v{version:apiVersion}/nationalparks")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    /*[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]*/
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository nationalParkRepository;
        private readonly IMapper mapper;

        public NationalParksController(INationalParkRepository nationalParkRepository, IMapper mapper)
        {
            this.nationalParkRepository = nationalParkRepository;
            this.mapper = mapper;
        }


        /// <summary>
        /// Get all national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var results = nationalParkRepository.GetNationalParks();
            var dtoResults = new List<NationalParkDto>();
            
            foreach(var nationalPark in results)
            {
                dtoResults.Add(mapper.Map<NationalParkDto>(nationalPark));
            }

            return Ok(dtoResults);
        }

        /// <summary>
        /// Get national park id depending the id
        /// </summary>
        /// <param name="nationalParkId"> National park id</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public IActionResult GetNationalPark(int nationalParkId)
        {
            var result = nationalParkRepository.GetNationalPark(nationalParkId);

            if (result == null)
                return NotFound();

            var dtoResult = mapper.Map<NationalParkDto>(result);

            return Ok(dtoResult);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesDefaultResponseType]
        public IActionResult CreateNationalPark([FromBody]NationalParkDto nationalParkDto)
        {
            if(nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (nationalParkRepository.NationalParkExists(nationalParkDto.Name))
            {
                ModelState.AddModelError("", "National Park already exists");
                return StatusCode(303, ModelState);
            } 

            var nationalPark = mapper.Map<NationalPark>(nationalParkDto);

            if (!nationalParkRepository.CreateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", $"There has been an error when creating {nationalPark.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationalPark", new { version=HttpContext.GetRequestedApiVersion().ToString(), 
                nationalParkId = nationalPark.Id }, nationalPark);
        }

        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody]NationalParkDto nationalParkDto)
        {
            if(nationalParkDto == null || nationalParkId != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            if (!nationalParkRepository.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }

            var nationalPark = mapper.Map<NationalPark>(nationalParkDto);

            if (!nationalParkRepository.UpdateNationalPark(nationalPark))
            {
                ModelState.AddModelError("", "Internal Server Error");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalParkId")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!nationalParkRepository.NationalParkExists(nationalParkId))
            {
                return NotFound();
            }

            var nationalPark = nationalParkRepository.GetNationalPark(nationalParkId);

            if (!nationalParkRepository.DeleteNationalPark(nationalPark))
            {
                ModelState.AddModelError("", "Internal server error");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

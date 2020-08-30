using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Route("api/v{version:apiVersion}/trails")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    /*[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecTrail")]*/
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository trailRepository;
        private readonly IMapper mapper;

        public TrailsController(ITrailRepository trailRepository, IMapper mapper)
        {
            this.trailRepository = trailRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TrailDto>))]
        public IActionResult GetTrails()
        {
            var results = trailRepository.GetTrails();
            var dtoResults = new List<TrailDto>();

            foreach (var trail in results)
            {
                dtoResults.Add(mapper.Map<TrailDto>(trail));
            }

            return Ok(dtoResults);
        }

        [HttpGet("{trailId:int}", Name = "GetTrail")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        [Authorize(Roles = "Admin")]
        public IActionResult GetTrail(int trailId)
        {
            var result = trailRepository.GetTrail(trailId);

            if (result == null)
            {
                return NotFound();
            }

            var dtoResult = mapper.Map<TrailDto>(result);

            return Ok(dtoResult);
        }

        [HttpGet("[action]/{nationalParkId:int}", Name = "GetTrailInNationalPark")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TrailDto))]
        public IActionResult GetTrailsInNationalPark(int nationalParkId)
        {
            var results = trailRepository.GetTrailsInNationalPark(nationalParkId);

            if (results == null)
            {
                return NotFound();
            }

            var dtoResults = new List<TrailDto>();

            foreach (var trail in results)
            {
                dtoResults.Add(mapper.Map<TrailDto>(trail));
            }

            return Ok(dtoResults);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        public IActionResult CreateTrail([FromBody]TrailCreateDto trailDto)
        {
            if (trailDto == null)
            {
                return BadRequest(ModelState);
            }

            var trail = mapper.Map<Trail>(trailDto);

            if (trailRepository.TrailExists(trail.Name))
            {
                ModelState.AddModelError("", "Trail already exists");
                return StatusCode(303, ModelState);
            }

            if (!trailRepository.CreateTrail(trail))
            {
                ModelState.AddModelError("", "Internal server error");
                return StatusCode(303, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { trailId = trail.Id }, trail);
        }

        [HttpPatch("{trailId:int}", Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult UpdateTrail(int trailId, [FromBody]TrailUpdateDto trailDto)
        {
            var trail = mapper.Map<Trail>(trailDto);

            if (!trailRepository.TrailExists(trailId))
            {
                return NotFound();
            }

            if (!trailRepository.UpdateTrail(trail))
            {
                ModelState.AddModelError("", "Internal server error");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{trailId:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public IActionResult DeleteTrail(int trailId)
        {
            if (!trailRepository.TrailExists(trailId))
            {
                return NotFound();
            }

            var trail = trailRepository.GetTrail(trailId);

            if (!trailRepository.DeleteTrail(trail))
            {
                ModelState.AddModelError("", "Internal server error");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

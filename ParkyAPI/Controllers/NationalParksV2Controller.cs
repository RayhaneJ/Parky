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
    [ApiVersion("2.0")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    /*[ApiExplorerSettings(GroupName = "ParkyOpenAPISpecNP")]*/
    public class NationalParksV2Controller : ControllerBase
    {
        private readonly INationalParkRepository nationalParkRepository;
        private readonly IMapper mapper;

        public NationalParksV2Controller(INationalParkRepository nationalParkRepository, IMapper mapper)
        {
            this.nationalParkRepository = nationalParkRepository;
            this.mapper = mapper;
        }


        /// <summary>
        /// Get all national parks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NationalParkDto>))]
        public IActionResult GetNationalParks()
        {
            var result = nationalParkRepository.GetNationalParks().FirstOrDefault();

            return Ok(mapper.Map<NationalParkDto>(result));
        }
    }
}

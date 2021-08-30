using CityInfo.API.Services;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        [HttpGet]
        public IActionResult GetCities()
        {
            var cityEntities = _cityInfoRepository.GetCities();

            return Ok(_mapper.Map<IEnumerable<CityWithoutPoIDto>>(cityEntities));
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePoI = false)
        {
            var city = _cityInfoRepository.GetCity(id, includePoI);

            if (city == null)
            {
                return NotFound();
            }
            if (includePoI)
            {
                return Ok(_mapper.Map<CityDto>(city));
            }

            return Ok(_mapper.Map<CityWithoutPoIDto>(city));
        }

    }
}

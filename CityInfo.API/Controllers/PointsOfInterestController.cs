using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly IMapper _mapper;
        private readonly ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger,
            IMailService mailService, IMapper mapper, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _cityInfoRepository = cityInfoRepository ?? throw new ArgumentNullException(nameof(cityInfoRepository));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                if (!_cityInfoRepository.CityExists(cityId))
                {
                    _logger.LogInformation($"City with id {cityId} was not found when accessing point of interest.");
                    return NotFound();
                }

                var poi = _cityInfoRepository.GetPointsOfInterests();

                return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(poi));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting points of interest for city with id {cityId}", ex);
                return StatusCode(500, "A problem happened while handling your request");
            }
        }



        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointsOfInterest(int cityId, int id)
        {
            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterest(cityId, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));

        }
        [HttpPost]
        public IActionResult CreatePointofInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Description cannot be the same as the name");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }


            var finalPointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterest);
            _cityInfoRepository.AddPointOfInterest(cityId, finalPointOfInterest);
            _cityInfoRepository.Save();

            var poiToReturn = _mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = poiToReturn.Id }, poiToReturn);

        }


        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Description == pointOfInterest.Name)
            {
                ModelState.AddModelError("Description", "Description cannot be the same as the name");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterest == null)
            {
                return NotFound();
            }


            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            _cityInfoRepository.UpdatePointOfInterest(cityId, pointOfInterestEntity);
            _cityInfoRepository.Save();
            return NoContent();


        }


        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Description == pointOfInterestToPatch.Name)
            {
                ModelState.AddModelError(
                    "Description",
                    "The provided description should be different");
            }
            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            _cityInfoRepository.UpdatePointOfInterest(cityId, pointOfInterestEntity);
            _cityInfoRepository.Save();

            return NoContent();


        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {

            if (!_cityInfoRepository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
            {
                return NotFound();
            }

            _cityInfoRepository.RemovePointOfInterest(pointOfInterestEntity);
            _cityInfoRepository.Save();

            _mailService.Send("Point of interest deleted.", $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted"
                );
            return NoContent();
        }
    }
}

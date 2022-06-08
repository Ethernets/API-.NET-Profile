﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkyAPI.Models;
using ParkyAPI.Models.Dtos;
using ParkyAPI.Repository.IRepository;

namespace ParkyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NationalParksController : ControllerBase
    {
        private INationalParkRepository _parkRepository;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository parkRepository, IMapper mapper)
        {
            _parkRepository = parkRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetNationalParks()
        {
            var objList = _parkRepository.GetNationalParks();

            var objDto = new List<NationalParkDto>();

            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }
            return Ok(objDto);
        }

        [HttpGet("{nationalParkId:int}")]
        public IActionResult GetNationalParks(int nationalParkId)
        {
            var obj = _parkRepository.GetNationalPark(nationalParkId);
            if (obj == null)
            {
                return NotFound();
            }
            var objDto = _mapper.Map<NationalParkDto>(obj);
            return Ok(objDto);
        }

        [HttpPost]
        public IActionResult CreateNationalPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_parkRepository.NationalParkExist(nationalParkDto.Name))
            {
              ModelState.AddModelError("", "Национальный парк существует");
              return StatusCode(404, ModelState);
            }

            if (!ModelState.IsValid )
            {
                return BadRequest(ModelState);

            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_parkRepository.CreateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Что-то пошло не так при сохранении записи {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return Ok();

        }
    }
}

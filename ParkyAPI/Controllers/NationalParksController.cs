using AutoMapper;
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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private INationalParkRepository _parkRepository;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository parkRepository, IMapper mapper)
        {
            _parkRepository = parkRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Вывод всех национальных парков
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(List<NationalParkDto>))]
        [ProducesResponseType(400)]
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

        /// <summary>
        /// Вывод нужного Вам парка по его ID 
        /// </summary>
        /// <param name="nationalParkId">Ввод ID национального парка</param>
        /// <returns></returns>
        [HttpGet("{nationalParkId:int}", Name = "GetNationalPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]
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

        /// <summary>
        /// Создание новых данных для национального парка
        /// </summary>
        /// <param name="nationalParkDto">Введите нужные данные</param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            return CreatedAtRoute("GetNationalPark", new {nationalParkId = nationalParkObj.Id}, nationalParkObj);
        }

        /// <summary>
        /// Обновление данных выбранного парка
        /// </summary>
        /// <param name="nationalParkId">Введите ID национального парка для его обновления</param>
        /// <returns></returns>
        [HttpPatch("{nationalParkId:int}", Name = "UpdateNationalPark")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateNationalPark(int nationalParkId, [FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null || nationalParkId != nationalParkDto.Id)
            {
                return BadRequest(ModelState);
            }

            var nationalParkObj = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_parkRepository.UpdateNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Что-то пошло не так при обновлении записи {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpDelete("{nationalParkId:int}", Name = "DeleteNationalPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteNationalPark(int nationalParkId)
        {
            if (!_parkRepository.NationalParkExist(nationalParkId))
            {
                return NotFound();
            }

            var nationalParkObj = _parkRepository.GetNationalPark(nationalParkId);

            if (!_parkRepository.DeleteNationalPark(nationalParkObj))
            {
                ModelState.AddModelError("", $"Что-то пошло не так при удалении записи {nationalParkObj.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

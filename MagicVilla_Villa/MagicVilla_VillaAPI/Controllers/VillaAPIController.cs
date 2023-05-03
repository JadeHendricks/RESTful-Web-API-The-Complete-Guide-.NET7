using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Controllers
{
    //ControllerBase for API, Controller for views and MVC extra features that we don't need
    //using [controller] inside of route will automatically use the className - "controller" if you want to do that.
    [Route("api/VillaAPI")]
    //this allows us to use see the model requirements aswell
    [ApiController]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        public VillaAPIController(ILogger<VillaAPIController> logger, ApplicationDbContext db, IMapper mapper, IVillaRepository dbVilla)
        {
            _logger = logger;
            _dbVilla = dbVilla;
            _mapper = mapper;
        }

        //If you do not add the HttpGet, then this will be the default anyways
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            _logger.LogInformation("Getting all Villas");
            IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
            //aka convert villaList to VillaDTO
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        //If you do not add the HttpGet, then this will be the default anyways
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                _logger.LogError("Get Villa Error with Id of " + id);
                return BadRequest();
            }

            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDTO>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody]VillaCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            //creating custom errors
            if (await _dbVilla.GetAsync(u => u.Name.ToLower() == createDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("CustomError", "Villa already Exists");
                return BadRequest(ModelState);
            }

            Villa model = _mapper.Map<Villa>(createDTO);

            //once the villa is created EFC will automatically add in the ID field for us here, which is why we don't need to specify it
            await _dbVilla.CreateAsync(model);
            //this will return the route at where the new entry has been created
            return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
        }

        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        //using IActionResult here allows us to not give a return type, as we are deleting something
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            //get the villa via Id
            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                return NotFound();
            }

            _dbVilla.RemoveAsync(villa);
            //with delete we usually use this method of return, but you could return whatever you want
            return NoContent();
        }

        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody]VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || id != updateDTO.Id)
            {
                return BadRequest();
            }

            Villa model = _mapper.Map<Villa>(updateDTO);


            //update will automatically update the changed properties
            await _dbVilla.UpdateAsync(model);

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //operation - What to do - generally "replace"
        //path - what field to update /fieldName
        //value - new value
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }

            //when you retreive a record, EF is tracking that, so if you do not want to track that we need to use AsNoTracking()
            //we don't want to make any changes to this object, we are using it to make a new object that we then pass back to the object
            //it will try and track both models with the same ID and that is not possible - we can only track 1 id at a time
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);

            if (villaDTO == null)
            {
                return BadRequest();
            }

            //if there are any errors it will be stored inside of the modelstate, which we will check
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villa);

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            return NoContent();
        }
    }
}

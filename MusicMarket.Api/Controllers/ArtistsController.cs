﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MusicMarket.Api.DTO;
using MusicMarket.Api.Validators;
using MusicMarket.Core.Models;
using MusicMarket.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicMarket.Api.Controllers
{
    [Route("api/[controller")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly IMapper _mapper;

        public ArtistsController(IArtistService artistService, IMapper mapper)
        {
            this._mapper = mapper;
            this._artistService = artistService;
        }

        [HttpGet("GetAllArtists")]
        public async Task<ActionResult<IEnumerable<ArtistDTO>>> GetAllArtists()
        {
            var artists = await _artistService.GetAllArtists();
            var artistResources = _mapper.Map<IEnumerable<Artist>, IEnumerable<ArtistDTO>>(artists);

            return Ok(artistResources);
        }
        [HttpGet("GetArtistById/{id}")]
        public async Task<ActionResult<ArtistDTO>> GetArtistById(int id)
        {
            var artist = await _artistService.GetArtistById(id);
            var artistResource = _mapper.Map<Artist, ArtistDTO>(artist);


            return Ok(artistResource);
        }
        [HttpPost("CreateArtist")]
        public async Task<ActionResult<ArtistDTO>> CreateArtist([FromBody] SaveArtistDTO saveArtistResource)
        {
            var validator = new SaveArtistResourceValidator();
            var validationResult = await validator.ValidateAsync(saveArtistResource);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);//this needs refining but for demo it is ok
            }

            var artistToCreate = _mapper.Map<SaveArtistDTO, Artist>(saveArtistResource);
            var newArtist = await _artistService.CreateArtist(artistToCreate);
            var artist = await _artistService.GetArtistById(newArtist.Id);
            var artistResource = _mapper.Map<Artist, ArtistDTO>(artist);
            return Ok(artistResource);
        }

        [HttpPut("UpdateArtist/{id}")]
        public async Task<ActionResult<ArtistDTO>> UpdateArtist(int id, [FromBody] SaveArtistDTO saveArtistResource)
        {
            var validator = new SaveArtistResourceValidator();
            var validationResult = await validator.ValidateAsync(saveArtistResource);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);//this needs refining but for demo it is ok
            }

            var artistToBeUpdated = await _artistService.GetArtistById(id);

            if (artistToBeUpdated == null)
            {
                return NotFound();
            }

            var artist = _mapper.Map<SaveArtistDTO, Artist>(saveArtistResource);
            await _artistService.UpdateArtist(artistToBeUpdated, artist);
            var updateArtist = await _artistService.GetArtistById(id);
            var updateArtistResource = _mapper.Map<Artist, ArtistDTO>(updateArtist);

            return Ok(updateArtistResource);
        }

        [HttpDelete("DeleteArtist/{id}")]
        public async Task<IActionResult> DeleteArtist(int id)
        {
            var artist = await _artistService.GetArtistById(id);

            await _artistService.DeleteArtist(artist);

            return NoContent();
        }
    }
}

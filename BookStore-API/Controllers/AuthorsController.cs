using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// End point for Author for book store
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _autherRepository;
        private readonly iLoggerService _logger;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository autherRepository,
            iLoggerService logger,
            IMapper mapper)
        {
            _autherRepository = autherRepository;
            _logger = logger;
            _mapper = mapper;
        }

        #region "Gets"
        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            try
            {
                _logger.LogInfo("Attempting Get Authors call");
                var authors = await _autherRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo("Success Get Authors call");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError("Error Get Authors call occurred: " + e.Message);
            }
        }

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            try
            {
                _logger.LogInfo($"Attempting Get Author call for id:{id}");
                var author = await _autherRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"Get Author call for id:{id} was not found.");
                    return NotFound();
                }
                var response = _mapper.Map<IList<AuthorDTO>>(author);
                _logger.LogInfo($"Success Get Author call for id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"Error Get Author call occurred for id:{id}: " + e.Message);
            }
        }
        #endregion

        #region "Create"
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            try
            {
                if(authorDTO == null)
                {
                    _logger.LogWarn($"Create author failed with empty author.");
                    return BadRequest(ModelState);
                }
                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"Create author failed with missing fields for author.");
                    return BadRequest(ModelState);
                }
                _logger.LogInfo($"Attempting Create Author call for {authorDTO.Firstname}-{authorDTO.Lastname}");
                var author = _mapper.Map<Author>(authorDTO);

                var isSuccess = await _autherRepository.Create(author);
                if(!isSuccess)
                {
                    return internalError($"Create author failed on save.");
                }

                return Created("Created", author);

                //var author = await _autherRepository.FindById(id);
                //if (author == null)
                //{
                //    _logger.LogWarn($"Get Author call for id:{id} was not found.");
                //    return NotFound();
                //}
                //var response = _mapper.Map<IList<AuthorDTO>>(author);
                //_logger.LogInfo($"Success Get Author call for id:{id}");
                //return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"Error Creating Author");
            }
        }
        #endregion

        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong");
        }
    }
}

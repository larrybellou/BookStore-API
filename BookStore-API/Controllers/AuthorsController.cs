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
        #region "Declarations"
        private readonly IAuthorRepository _autherRepository;
        private readonly iLoggerService _logger;
        private readonly IMapper _mapper;
        #endregion

        #region "Constructor"
        public AuthorsController(IAuthorRepository autherRepository,
            iLoggerService logger,
            IMapper mapper)
        {
            _autherRepository = autherRepository;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

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
                var response = _mapper.Map<AuthorDTO>(author);
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
        /// <summary>
        /// Creates an author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

                return Created($"https://localhost:44307/api/Authors/{author.Id}", author);

            }
            catch (Exception e)
            {
                return internalError($"Error Creating Author: {e.Message}");
            }
        }
        #endregion

        #region "Update"
        /// <summary>
        /// Updates an author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            try
            {
                if (id < 1 || authorDTO == null || authorDTO.Id != id)
                {
                    _logger.LogWarn($"Update author failed with empty author.");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Update author failed with missing fields for author.");
                    return BadRequest(ModelState);
                }
                var isExists = await _autherRepository.isExists(id);
                if (isExists == false)
                {
                    return NotFound();
                }
                _logger.LogInfo($"Attempting Update Author call for {id}-{authorDTO.Firstname}-{authorDTO.Lastname}");
                var author = _mapper.Map<Author>(authorDTO);

                var isSuccess = await _autherRepository.Update(author);
                if (!isSuccess)
                {
                    return internalError($"Update author failed on save.");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"Error Updating Author: {e.Message}");
            }
        }
        #endregion

        #region "Delete"
        /// <summary>
        /// Deletes an author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1)
                {
                    _logger.LogWarn($"Delete author failed with empty author.");
                    return BadRequest();
                }
                var author = await _autherRepository.FindById(id);
                if(author == null)
                {
                    return NotFound();
                }
                _logger.LogInfo($"Attempting Delete Author call for {id}-{author.Firstname} {author.Lastname}");

                var isSuccess = await _autherRepository.Delete(author);
                if (!isSuccess)
                {
                    return internalError($"Delete author failed on save.");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"Error Deleting Author: {e.Message}");
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

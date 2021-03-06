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
        private readonly IAuthorRepository _authorRepository;
        private readonly iLoggerService _logger;
        private readonly IMapper _mapper;
        #endregion

        #region "Constructor"
        public AuthorsController(IAuthorRepository authorRepository,
            iLoggerService logger,
            IMapper mapper)
        {
            _authorRepository = authorRepository;
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
                _logger.LogInfo($"{GetCallerNames()}: Attempting");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{GetCallerNames()}: Success");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred: " + e.Message);
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
                _logger.LogInfo($"{GetCallerNames()}: Attempting call for id:{id}");
                var author = await _authorRepository.FindById(id);
                if (author == null)
                {
                    _logger.LogWarn($"{GetCallerNames()}: Not found for id {id}.");
                    return NotFound();
                }
                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{GetCallerNames()}: Success for id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred for id:{id}: " + e.Message);
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
                    _logger.LogWarn($"{GetCallerNames()}: failed with empty.");
                    return BadRequest(ModelState);
                }
                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"{GetCallerNames()}: failed with missing fields.");
                    return BadRequest(ModelState);
                }
                _logger.LogInfo($"{GetCallerNames()}: Attempting for {authorDTO.Firstname}-{authorDTO.Lastname}");
                var author = _mapper.Map<Author>(authorDTO);

                var isSuccess = await _authorRepository.Create(author);
                if (isSuccess)
                {
                    _logger.LogInfo($"{GetCallerNames()}: Success for {author.Id}: {authorDTO.Firstname}-{authorDTO.Lastname}");
                }
                else 
                { 
                    return internalError($"{GetCallerNames()}: failed on save.");
                }

                return Created($"https://localhost:44307/api/Authors/{author.Id}", author);

            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred: {e.Message}");
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
                    _logger.LogWarn($"{GetCallerNames()}: failed with missing fields.");
                    return BadRequest(ModelState);
                }
                var isExists = await _authorRepository.isExists(id);
                if (isExists == false)
                {
                    return NotFound();
                }
                _logger.LogInfo($"{GetCallerNames()}: Attempting for {id}-{authorDTO.Firstname}-{authorDTO.Lastname}");
                var author = _mapper.Map<Author>(authorDTO);

                var isSuccess = await _authorRepository.Update(author);
                if (isSuccess)
                {
                    _logger.LogInfo($"{GetCallerNames()}: Success for {author.Id}: {authorDTO.Firstname}-{authorDTO.Lastname}");
                }
                else
                {
                    return internalError($"{GetCallerNames()}: failed on save.");
                }


                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred: {e.Message}");
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
                    _logger.LogWarn($"{GetCallerNames()}: failed with empty.");
                    return BadRequest();
                }
                var author = await _authorRepository.FindById(id);
                if(author == null)
                {
                    return NotFound();
                }
                _logger.LogInfo($"{GetCallerNames()}: Attempting for {id}-{author.Firstname} {author.Lastname}");

                var isSuccess = await _authorRepository.Delete(author);
                if (isSuccess)
                {
                    _logger.LogInfo($"{GetCallerNames()}: Success for {author.Id}: {author.Firstname}-{author.Lastname}");
                }
                else
                {
                    return internalError($"{GetCallerNames()}: failed on save.");
                }


                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred: {e.Message}");
            }
        }
        #endregion

        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong");
        }
        private string GetCallerNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;
            return $"{controller}-{action}";
        }
    }
}

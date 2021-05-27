using AutoMapper;
using BookStore_API.Contracts;
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

        /// <summary>
        /// Get all authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
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

        private ObjectResult internalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong");
        }
    }
}

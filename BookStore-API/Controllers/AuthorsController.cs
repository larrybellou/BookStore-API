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
    public class AuthorsController: ControllerBase
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
                _logger.LogError("Error Get Authors call occurred: " + e.Message);
                return StatusCode(500, "Something went wrong, contact administrator");
            }


        }
    }
}

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
    /// End point for Book for book store
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public class BooksController : ControllerBase
    {
        #region "Declarations"
        private readonly IBookRepository _bookRepository;
        private readonly iLoggerService _logger;
        private readonly IMapper _mapper;
        #endregion

        #region "Constructor"
        public BooksController(IBookRepository bookRepository,
            iLoggerService logger,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }
        #endregion

        #region "Gets"
        /// <summary>
        /// Get all Books
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                _logger.LogInfo($"{GetCallerNames()}: Attempting");
                var Books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(Books);
                _logger.LogInfo($"{GetCallerNames()}: Success");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred: " + e.Message);
            }
        }

        /// <summary>
        /// Get all Books
        /// </summary>
        /// <returns>List of Books</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                _logger.LogInfo($"{GetCallerNames()}: Attempting for id:{id}");
                var Book = await _bookRepository.FindById(id);
                if (Book == null)
                {
                    _logger.LogWarn($"{GetCallerNames()}: Not found for {id}.");
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(Book);
                _logger.LogInfo($"{GetCallerNames()}: Success for id:{id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error for id:{id}: " + e.Message);
            }
        }
        #endregion

        #region "Create"
        /// <summary>
        /// Creates an Book
        /// </summary>
        /// <param name="BookDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO BookDTO)
        {
            try
            {
                if(BookDTO == null)
                {
                    _logger.LogWarn($"{GetCallerNames()}: failed because empty.");
                    return BadRequest(ModelState);
                }
                if(!ModelState.IsValid)
                {
                    _logger.LogWarn($"{GetCallerNames()}: failed with missing fields.");
                    return BadRequest(ModelState);
                }
                _logger.LogInfo($"{GetCallerNames()}: Attempting for {BookDTO.Title}-{BookDTO.Summary}");
                var Book = _mapper.Map<Book>(BookDTO);

                var isSuccess = await _bookRepository.Create(Book);
                if (isSuccess)
                {
                    _logger.LogInfo($"{GetCallerNames()}: Success for {Book.Id}: {Book.Title}-{Book.Summary}");
                }
                else
                {
                    return internalError($"{GetCallerNames()}: failed on save.");
                }

                return Created($"https://localhost:44307/api/Books/{Book.Id}", Book);

            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error occurred: " + e.Message);
            }
        }
        #endregion

        #region "Update"
        /// <summary>
        /// Updates an Book
        /// </summary>
        /// <param name="BookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO BookDTO)
        {
            try
            {
                if (id < 1 || BookDTO == null || BookDTO.Id != id)
                {
                    _logger.LogWarn($"{GetCallerNames()}: failed with empty.");
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{GetCallerNames()}: failed with missing fields.");
                    return BadRequest(ModelState);
                }
                var isExists = await _bookRepository.isExists(id);
                if (isExists == false)
                {
                    return NotFound();
                }
                _logger.LogInfo($"{GetCallerNames()}: Attempting for {id}-{BookDTO.Title}-{BookDTO.Summary}");
                var Book = _mapper.Map<Book>(BookDTO);

                var isSuccess = await _bookRepository.Update(Book);
                if (isSuccess)
                {
                    _logger.LogInfo($"{GetCallerNames()}: Success for {id}:{Book.Title}-{Book.Summary}");
                }
                else
                {
                    return internalError($"{GetCallerNames()}: failed on save.");
                }
                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error for id:{id}: " + e.Message);
            }
        }
        #endregion

        #region "Delete"
        /// <summary>
        /// Deletes an Book
        /// </summary>
        /// <param name="BookDTO"></param>
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
                var Book = await _bookRepository.FindById(id);
                if(Book == null)
                {
                    return NotFound();
                }
                _logger.LogInfo($"{GetCallerNames()}: Attempting for {id}-{Book.Title} {Book.Summary}");

                var isSuccess = await _bookRepository.Delete(Book);
                if (isSuccess)
                {
                    _logger.LogInfo($"{GetCallerNames()}: Success for {id}: {Book.Title}-{Book.Summary}");
                }
                else
                {
                    return internalError($"{GetCallerNames()}: failed on save.");
                }

                return NoContent();
            }
            catch (Exception e)
            {
                return internalError($"{GetCallerNames()}: Error for id:{id}: " + e.Message);
            }
        }
        #endregion

        private ObjectResult internalError(string message)
        {
            _logger.LogError($"{message}");
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookCatalogAPI.Contracts;
using BookCatalogAPI.Data;
using BookCatalogAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;

namespace BookCatalogAPI.Controllers
{
    /// <summary>
    /// Контроллер книг
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BooksController(IBookRepository bookRepository,
            IWebHostEnvironment env,
            IMapper mapper)
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _env = env;
        }

        private string GetImagePath(string fileName)
            => ($"{_env.ContentRootPath}\\uploads\\{fileName}");


        /// <summary>
        /// Get All Books
        /// </summary>
        /// <returns>A List of Books</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            try
            {
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                foreach (var item in response)
                {
                    if (!string.IsNullOrEmpty(item.Image))
                    {
                        var imgPath = GetImagePath(item.Image);
                        if (System.IO.File.Exists(imgPath))
                        {
                            byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
                            item.File = Convert.ToBase64String(imgBytes);
                        }
                    }
                }
                
                return Ok(response);
            }
            catch (Exception )
            {
                return InternalError();
            }
        }
        /// <summary>
        /// Gets a Book by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A Book record</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                
                var book = await _bookRepository.FindById(id);
                if (book == null)
                {
                    return NotFound();
                }
                var response = _mapper.Map<BookDTO>(book);
                if (!string.IsNullOrEmpty(response.Image))
                {
                    var imgPath = GetImagePath(book.Image);
                    if (System.IO.File.Exists(imgPath))
                    {
                        byte[] imgBytes = System.IO.File.ReadAllBytes(imgPath);
                        response.File = Convert.ToBase64String(imgBytes);
                    }
                }

                return Ok(response);
            }
            catch (Exception)
            {
                return InternalError();
            }
        }
        /// <summary>
        /// Creates a new book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns>Book Object</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            try
            {
                if (bookDTO == null)
                {
                    return BadRequest(ModelState);
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Create(book);
                if (!isSuccess)
                {
                    return InternalError();
                }
                if (!string.IsNullOrEmpty(bookDTO.File))
                {
                    var imgPath = GetImagePath(bookDTO.Image);
                    byte[] imageBytes = Convert.FromBase64String(bookDTO.File);
                    System.IO.File.WriteAllBytes(imgPath, imageBytes);
                }
                
                return Created("Create", new { book });
            }
            catch (Exception)
            {
                return InternalError();
            }
        }
        /// <summary>
        /// Update a Book by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            try
            {
                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    return BadRequest();
                }
                var isExists = await _bookRepository.IsExists(id);

                if (!isExists)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var oldImage = await _bookRepository.GetImageFileName(id);
                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Update(book);
                if (!isSuccess)
                {
                    return InternalError();
                }

                if (!bookDTO.Image.Equals(oldImage))
                {
                    if (System.IO.File.Exists(GetImagePath(oldImage)))
                    {
                        System.IO.File.Delete(GetImagePath(oldImage));
                    }
                }

                if (!string.IsNullOrEmpty(bookDTO.File))
                {
                    byte[] imageBytes = Convert.FromBase64String(bookDTO.File);
                    await System.IO.File.WriteAllBytesAsync(GetImagePath(bookDTO.Image), imageBytes);
                }

                return NoContent();
            }
            catch (Exception)
            {
                return InternalError();
            }
        }

        /// <summary>
        /// Removes an book by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id < 1)
                {
                    return BadRequest();
                }
                var isExists = await _bookRepository.IsExists(id);
                if (!isExists)
                {
                    return NotFound();
                }
                var book = await _bookRepository.FindById(id);
                var isSuccess = await _bookRepository.Delete(book);
                if (!isSuccess)
                {
                    return InternalError();
                }
                return NoContent();
            }
            catch (Exception)
            {
                return InternalError();
            }
        }

        private ObjectResult InternalError()
        {
            return StatusCode(500, "Something went wrong. Please contact the Administrator");
        }
    }
}

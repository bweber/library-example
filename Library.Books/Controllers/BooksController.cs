using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Library.Books.Models;
using Library.Books.Services;
using Library.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Books.Controllers
{
    [Route("books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BooksController(IBookService bookService)
        {
            _bookService = bookService;
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var book = await _bookService.GetById(id);
            if (book == null)
                return NotFound();
            
            return Ok(book);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] BookModel bookModel)
        {
            if (bookModel.Id.HasValue)
            {
                var book = await _bookService.GetById(bookModel.Id.Value);
                if (book != null)
                {
                    return new SeeOtherStatusCode(nameof(GetById), null, new { id = bookModel.Id });
                }
            }

            var result = await _bookService.Create(bookModel);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Save(Guid id, [FromBody] BookModel bookModel)
        {
            var book = await _bookService.GetById(id);
            if (book == null)
                return NotFound();
            
            bookModel.Id = id;
            var result = await _bookService.Update(bookModel);

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _bookService.GetById(id);
            if (author == null)
                return NotFound();
            
            await _bookService.Delete(id);

            return NoContent();
        }
    }
}
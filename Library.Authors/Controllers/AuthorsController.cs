using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Library.Authors.Models;
using Library.Authors.Services;
using Library.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Authors.Controllers
{
    [Route("authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var author = await _authorService.GetById(id);
            if (author == null)
                return NotFound();
            
            return Ok(author);
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AuthorModel authorModel)
        {
            if (authorModel.Id.HasValue)
            {
                var author = await _authorService.GetById(authorModel.Id.Value);
                if (author != null)
                {
                    return new SeeOtherStatusCode(nameof(GetById), null, new { id = authorModel.Id });
                }
            }

            var result = await _authorService.Create(authorModel);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        
        [HttpPut("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Save(Guid id, [FromBody] AuthorModel authorModel)
        {
            var author = await _authorService.GetById(id);
            if (author == null)
                return NotFound();

            authorModel.Id = id;
            var result = await _authorService.Update(authorModel);

            return Ok(result);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var author = await _authorService.GetById(id);
            if (author == null)
                return NotFound();
            
            await _authorService.Delete(id);

            return NoContent();
        }
    }
}
using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Library.Books.Errors;
using Library.Books.Handlers.Commands;
using Library.Books.Handlers.Queries;
using Library.Books.Models;
using Library.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Books.Controllers
{
    [Route("books")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetBookById.Query(id), cancellationToken);

            return result.Match<ActionResult>(
                Ok,
                error => error switch
                {
                    BookNotFound => NotFound(),
                    _ => Problem()
                });
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookModel model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateBook.Command(model), cancellationToken);

            return result.Match<IActionResult>(
                book => CreatedAtAction(nameof(GetById), new { id = book.Id }, book),
                error => error switch
                {
                    BookExistsError e => new SeeOtherStatusCode(nameof(GetById), null, new { id = e.BookId }),
                    _ => Problem()
                });
        }

        [HttpPut("{id:guid}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] BookModel model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateBook.Command(id, model), cancellationToken);

            return result.Match<ActionResult>(
                Ok,
                error => error switch
                {
                    BookNotFound => NotFound(),
                    _ => Problem()
                });
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteBook.Command(id), cancellationToken);

            return result.Match<ActionResult>(
                _ => NoContent(),
                error => error switch
                {
                    BookNotFound => NotFound(),
                    _ => Problem()
                });
        }
    }
}

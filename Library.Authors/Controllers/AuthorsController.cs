using System;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Library.Authors.Errors;
using Library.Authors.Handlers.Commands;
using Library.Authors.Handlers.Queries;
using Library.Authors.Models;
using Library.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Library.Authors.Controllers
{
    [Route("authors")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAuthorById.Query(id), cancellationToken);

            return result.Match<ActionResult>(
                Ok,
                error => error switch
                {
                    AuthorNotFound => NotFound(),
                    _ => Problem()
                });
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorModel model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateAuthor.Command(model), cancellationToken);

            return result.Match<IActionResult>(
                author => CreatedAtAction(nameof(GetById), new { id = author.Id }, author),
                error => error switch
                {
                    AuthorExistsError e => new SeeOtherStatusCode(nameof(GetById), null, new { id = e.AuthorId }),
                    _ => Problem()
                });
        }

        [HttpPut("{id:guid}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] AuthorModel model, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateAuthor.Command(id, model), cancellationToken);

            return result.Match<ActionResult>(
                Ok,
                error => error switch
                {
                    AuthorNotFound => NotFound(),
                    _ => Problem()
                });
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteAuthor.Command(id), cancellationToken);

            return result.Match<ActionResult>(
                _ => NoContent(),
                error => error switch
                {
                    AuthorNotFound => NotFound(),
                    _ => Problem()
                });
        }
    }
}

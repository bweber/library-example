using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Library.Authors.Errors;
using Library.Common.Data;
using Library.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unit = LanguageExt.Unit;

namespace Library.Authors.Handlers.Commands
{
    public class DeleteAuthor : IRequestHandler<DeleteAuthor.Command, Either<Error, Unit>>
    {
        public record Command(
            Guid AuthorId
        ) : IRequest<Either<Error, Unit>>;

        private readonly LibraryDBContext _dbContext;
        private readonly ILogger<DeleteAuthor> _logger;

        public DeleteAuthor(LibraryDBContext dbContext, ILogger<DeleteAuthor> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Either<Error, Unit>> Handle(Command command, CancellationToken cancellationToken)
        {
            var author = await _dbContext.Authors
                .SingleOrDefaultAsync(a => a.Id == command.AuthorId, cancellationToken);
            if (author == null)
            {
                _logger.LogWarning("Could not find author by ID {AuthorId}", command.AuthorId);
                return new AuthorNotFound(command.AuthorId);
            }

            _dbContext.Remove(author);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Author deleted by ID {AuthorId}", command.AuthorId);

            return Unit.Default;
        }
    }
}

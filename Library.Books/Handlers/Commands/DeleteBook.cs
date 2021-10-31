using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt;
using Library.Books.Errors;
using Library.Common.Data;
using Library.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Unit = LanguageExt.Unit;

namespace Library.Books.Handlers.Commands
{
    public class DeleteBook : IRequestHandler<DeleteBook.Command, Either<Error, Unit>>
    {
        public record Command(
            Guid BookId
        ) : IRequest<Either<Error, Unit>>;

        private readonly LibraryDBContext _dbContext;
        private readonly ILogger<DeleteBook> _logger;

        public DeleteBook(LibraryDBContext dbContext, ILogger<DeleteBook> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Either<Error, Unit>> Handle(Command command, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Books
                .SingleOrDefaultAsync(a => a.Id == command.BookId, cancellationToken);
            if (book == null)
            {
                _logger.LogWarning("Could not find book by ID {BookId}", command.BookId);
                return new BookNotFound(command.BookId);
            }

            _dbContext.Remove(book);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Book deleted by ID {BookId}", command.BookId);

            return Unit.Default;
        }
    }
}

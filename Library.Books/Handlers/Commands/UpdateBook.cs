using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Library.Books.Errors;
using Library.Books.Models;
using Library.Common.Data;
using Library.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Books.Handlers.Commands
{
    public class UpdateBook : IRequestHandler<UpdateBook.Command, Either<Error, BookModel>>
    {
        public record Command(
            Guid BookId,
            BookModel Request
        ) : IRequest<Either<Error, BookModel>>;

        private readonly LibraryDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateBook> _logger;

        public UpdateBook(LibraryDBContext dbContext, IMapper mapper, ILogger<UpdateBook> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Either<Error, BookModel>> Handle(Command command, CancellationToken cancellationToken)
        {
            var (bookId, model) = command;

            var book = await _dbContext.Books
                .SingleOrDefaultAsync(a => a.Id == bookId, cancellationToken);
            if (book == null)
            {
                _logger.LogWarning("Could not find book by ID {BookId}", bookId);
                return new BookNotFound(bookId);
            }

            book.Subject = model.Subject;
            book.Title = model.Title;
            book.AuthorId = model.AuthorId.GetValueOrDefault();

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Book updated by ID {BookId}", bookId);

            return _mapper.Map<BookModel>(book);
        }
    }
}

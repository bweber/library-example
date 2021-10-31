using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Library.Books.Errors;
using Library.Books.Models;
using Library.Common.Data;
using Library.Common.Data.Entities;
using Library.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Books.Handlers.Commands
{
    public class CreateBook : IRequestHandler<CreateBook.Command, Either<Error, BookModel>>
    {
        public record Command(
            BookModel Request
        ) : IRequest<Either<Error, BookModel>>;

        private readonly LibraryDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateBook> _logger;

        public CreateBook(LibraryDBContext dbContext, IMapper mapper, ILogger<CreateBook> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Either<Error, BookModel>> Handle(Command command, CancellationToken cancellationToken)
        {
            var model = command.Request;

            if (model.Id.HasValue)
            {
                var bookId = model.Id.Value;
                var isExisting = await _dbContext.Books.AnyAsync(x => x.Id == bookId, cancellationToken);
                if (isExisting)
                {
                    _logger.LogWarning("Book exists for ID {BookId}", bookId);
                    return new BookExistsError(bookId);
                }
            }
            else
                model.Id = Guid.NewGuid();

            var book = _mapper.Map<Book>(model);

            await _dbContext.AddAsync(book, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Book created for ID {BookId}", model.Id);

            return model;
        }
    }
}

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

namespace Library.Books.Handlers.Queries
{
    public class GetBookById : IRequestHandler<GetBookById.Query, Either<Error, BookModel>>
    {
        public record Query(
            Guid BookId
        ) : IRequest<Either<Error, BookModel>>;

        private readonly LibraryDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetBookById> _logger;

        public GetBookById(LibraryDBContext dbContext, IMapper mapper, ILogger<GetBookById> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Either<Error, BookModel>> Handle(Query query, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Books.SingleOrDefaultAsync(x => x.Id == query.BookId, cancellationToken);
            if (book == null)
            {
                _logger.LogWarning("Book not found for ID {BookId}", query.BookId);
                return new BookNotFound(query.BookId);
            }

            _logger.LogInformation("Book found for ID {BookId}", query.BookId);
            return _mapper.Map<BookModel>(book);
        }
    }
}

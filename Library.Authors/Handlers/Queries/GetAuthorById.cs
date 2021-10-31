using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Library.Authors.Errors;
using Library.Authors.Models;
using Library.Common.Data;
using Library.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Authors.Handlers.Queries
{
    public class GetAuthorById : IRequestHandler<GetAuthorById.Query, Either<Error, AuthorModel>>
    {
        public record Query(
            Guid AuthorId
        ) : IRequest<Either<Error, AuthorModel>>;

        private readonly LibraryDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAuthorById> _logger;

        public GetAuthorById(LibraryDBContext dbContext, IMapper mapper, ILogger<GetAuthorById> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Either<Error, AuthorModel>> Handle(Query query, CancellationToken cancellationToken)
        {
            var book = await _dbContext.Authors.SingleOrDefaultAsync(x => x.Id == query.AuthorId, cancellationToken);
            if (book == null)
            {
                _logger.LogWarning("Author not found for ID {AuthorId}", query.AuthorId);
                return new AuthorNotFound(query.AuthorId);
            }

            _logger.LogInformation("Author found for ID {AuthorId}", query.AuthorId);
            return _mapper.Map<AuthorModel>(book);
        }
    }
}

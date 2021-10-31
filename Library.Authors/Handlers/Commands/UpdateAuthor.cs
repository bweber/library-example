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

namespace Library.Authors.Handlers.Commands
{
    public class UpdateAuthor : IRequestHandler<UpdateAuthor.Command, Either<Error, AuthorModel>>
    {
        public record Command(
            Guid AuthorId,
            AuthorModel Request
        ) : IRequest<Either<Error, AuthorModel>>;

        private readonly LibraryDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateAuthor> _logger;

        public UpdateAuthor(LibraryDBContext dbContext, IMapper mapper, ILogger<UpdateAuthor> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Either<Error, AuthorModel>> Handle(Command command, CancellationToken cancellationToken)
        {
            var (authorId, model) = command;

            var author = await _dbContext.Authors
                .SingleOrDefaultAsync(a => a.Id == authorId, cancellationToken);
            if (author == null)
            {
                _logger.LogWarning("Could not find author by ID {AuthorId}", authorId);
                return new AuthorNotFound(authorId);
            }

            author.FirstName = model.FirstName;
            author.LastName = model.LastName;

            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Author updated by ID {AuthorId}", authorId);

            return _mapper.Map<AuthorModel>(author);
        }
    }
}

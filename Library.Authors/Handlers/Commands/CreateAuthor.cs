using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using LanguageExt;
using Library.Authors.Errors;
using Library.Authors.Models;
using Library.Common.Data;
using Library.Common.Data.Entities;
using Library.Common.Errors;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Library.Authors.Handlers.Commands
{
    public class CreateAuthor : IRequestHandler<CreateAuthor.Command, Either<Error, AuthorModel>>
    {
        public record Command(
            AuthorModel Request
        ) : IRequest<Either<Error, AuthorModel>>;

        private readonly LibraryDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateAuthor> _logger;

        public CreateAuthor(LibraryDBContext dbContext, IMapper mapper, ILogger<CreateAuthor> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Either<Error, AuthorModel>> Handle(Command command, CancellationToken cancellationToken)
        {
            var model = command.Request;

            if (model.Id.HasValue)
            {
                var authorId = model.Id.Value;
                var isExisting = await _dbContext.Authors.AnyAsync(x => x.Id == authorId, cancellationToken);
                if (isExisting)
                {
                    _logger.LogWarning("Author exists for ID {AuthorId}", authorId);
                    return new AuthorExistsError(authorId);
                }
            }
            else
                model.Id = Guid.NewGuid();

            var book = _mapper.Map<Author>(model);

            await _dbContext.AddAsync(book, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Author created for ID {AuthorId}", model.Id);

            return model;
        }
    }
}

using System;
using Library.Common.Errors;

namespace Library.Authors.Errors
{
    public record AuthorExistsError(Guid AuthorId) : Error;
}

using System;
using Library.Common.Errors;

namespace Library.Authors.Errors
{
    public record AuthorNotFound(Guid AuthorId) : Error;
}

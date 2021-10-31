using System;
using Library.Common.Errors;

namespace Library.Books.Errors
{
    public record BookNotFound(Guid BookId) : Error;
}

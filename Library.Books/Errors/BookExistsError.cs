using System;
using Library.Common.Errors;

namespace Library.Books.Errors
{
    public record BookExistsError(Guid BookId) : Error;
}

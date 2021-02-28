using System;
using System.Linq;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.Validators;
using Library.Common.Data;

namespace Library.Books.Models
{
    public class BookModel
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("subject")]
        public string Subject { get; set; }
        
        [JsonPropertyName("author_id")]
        public Guid? AuthorId { get; set; }
    }
    
    public class BookModelValidator : AbstractValidator<BookModel> 
    {
        public BookModelValidator(LibraryDBContext context) 
        {
            RuleFor(x => x.Title)
                .NotNull().WithMessage("title is required")
                .MaximumLength(250).WithMessage("title has a maximum length of 250");
            
            RuleFor(x => x.Subject)
                .NotNull().WithMessage("subject is required")
                .MaximumLength(250).WithMessage("subject has a maximum length of 250");
            
            RuleFor(x => x.AuthorId)
                .NotNull().WithMessage("author_id is required")
                .SetValidator(new AuthorValidator(context)).WithMessage("author_id does not reference an existing author");
        }
    }
    
    public class AuthorValidator : PropertyValidator
    {
        private readonly LibraryDBContext _context;

        public AuthorValidator(LibraryDBContext context)
        {
            _context = context;
        }
        
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var authorId = (Guid?) context.PropertyValue;
            return _context.Authors.Any(x => x.Id == authorId);
        }
    }
}
using System;
using System.Text.Json.Serialization;
using FluentValidation;

namespace Library.Authors.Models
{
    public class AuthorModel
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
    
    public class AuthorModelValidator : AbstractValidator<AuthorModel> 
    {
        public AuthorModelValidator() 
        {
            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("first_name is required")
                .MaximumLength(100).WithMessage("first_name has a maximum length of 100");
            
            RuleFor(x => x.LastName)
                .NotNull().WithMessage("last_name is required")
                .MaximumLength(100).WithMessage("last_name has a maximum length of 100");
        }
    }
}
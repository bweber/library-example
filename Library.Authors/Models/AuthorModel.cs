using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Library.Authors.Models
{
    public class AuthorModel
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; set; }
        
        [Required(ErrorMessage = "first_name is required")]
        [MaxLength(100, ErrorMessage = "first_name has a maximum length of 100")]
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        
        [Required(ErrorMessage = "last_name is required")]
        [MaxLength(100, ErrorMessage = "last_name has a maximum length of 100")]
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
}
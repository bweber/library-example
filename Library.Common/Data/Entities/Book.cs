using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Common.Data.Entities
{
    [Table("Books")]
    public class Book
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required, MaxLength(250)]
        public string Title { get; set; }
        
        [Required, MaxLength(250)]
        public string Subject { get; set; }
        
        [Required, MaxLength(250)]
        public Guid AuthorId { get; set; }
        
        public Author Author { get; set; }
    }
}
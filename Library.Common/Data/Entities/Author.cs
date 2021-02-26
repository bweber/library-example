using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Common.Data.Entities
{
    [Table("Authors")]
    public class Author
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        
        [Required, MaxLength(100)]
        public string LastName { get; set; }
        
        public IEnumerable<Book> Books { get; } = new List<Book>();
        
        internal static void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.HasMany(b => b.Books).WithOne(b => b.Author).HasForeignKey(b => b.AuthorId);
        }
    }
}
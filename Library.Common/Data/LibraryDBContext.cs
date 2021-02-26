using Library.Common.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.Common.Data
{
    public class LibraryDBContext: DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        
        public LibraryDBContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>(Author.Configure);
        }
    }
}
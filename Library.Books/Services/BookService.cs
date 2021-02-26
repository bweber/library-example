using System;
using System.Threading.Tasks;
using AutoMapper;
using Library.Books.Models;
using Library.Common.Data;
using Library.Common.Data.Entities;

namespace Library.Books.Services
{
    public interface IBookService
    {
        Task<BookModel> GetById(Guid id);
        Task<BookModel> Create(BookModel bookModel);
        Task<BookModel> Update(BookModel bookModel);
        Task Delete(Guid id);
    }
    
    public class BookService : IBookService
    {
        private readonly LibraryDBContext _context;
        private readonly IMapper _mapper;

        public BookService(LibraryDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BookModel> GetById(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            return book == null ? null : _mapper.Map<BookModel>(book);
        }

        public async Task<BookModel> Create(BookModel bookModel)
        {
            bookModel.Id ??= Guid.NewGuid();
            
            var book = _mapper.Map<Book>(bookModel);

            await _context.AddAsync(book);
            await _context.SaveChangesAsync();
            
            return bookModel;
        }

        public async Task<BookModel> Update(BookModel bookModel)
        {
            var existing = await _context.Books.FindAsync(bookModel.Id);
            
            existing.Subject = bookModel.Subject;
            existing.Title = bookModel.Title;
            existing.AuthorId = bookModel.AuthorId.GetValueOrDefault();
            
            await _context.SaveChangesAsync();
            
            return bookModel;
        }

        public async Task Delete(Guid id)
        {
            var existing = await _context.Books.FindAsync(id);

            _context.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
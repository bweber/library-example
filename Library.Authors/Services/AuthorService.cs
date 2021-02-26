using System;
using System.Threading.Tasks;
using AutoMapper;
using Library.Authors.Models;
using Library.Common.Data;
using Library.Common.Data.Entities;

namespace Library.Authors.Services
{
    public interface IAuthorService
    {
        Task<AuthorModel> GetById(Guid id);
        Task<AuthorModel> Create(AuthorModel authorModel);
        Task<AuthorModel> Update(AuthorModel authorModel);
        Task Delete(Guid id);
    }
    
    public class AuthorService : IAuthorService
    {
        private readonly LibraryDBContext _context;
        private readonly IMapper _mapper;

        public AuthorService(LibraryDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<AuthorModel> GetById(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            return author == null ? null : _mapper.Map<AuthorModel>(author);
        }

        public async Task<AuthorModel> Create(AuthorModel authorModel)
        {
            authorModel.Id ??= Guid.NewGuid();
            
            var author = _mapper.Map<Author>(authorModel);

            await _context.AddAsync(author);
            await _context.SaveChangesAsync();
            
            return authorModel;
        }

        public async Task<AuthorModel> Update(AuthorModel authorModel)
        {
            var existing = await _context.Authors.FindAsync(authorModel.Id);
            
            existing.FirstName = authorModel.FirstName;
            existing.LastName = authorModel.LastName;
            
            await _context.SaveChangesAsync();
            
            return authorModel;
        }

        public async Task Delete(Guid id)
        {
            var existing = await _context.Authors.FindAsync(id);

            _context.Remove(existing);
            await _context.SaveChangesAsync();
        }
    }
}
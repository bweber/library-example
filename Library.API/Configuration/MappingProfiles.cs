using AutoMapper;
using Library.Authors.Models;
using Library.Books.Models;
using Library.Common.Data.Entities;

namespace Library.API.Configuration
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Book, BookModel>().ReverseMap();
            CreateMap<Author, AuthorModel>().ReverseMap();
        }
    }
}
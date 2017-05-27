using AutoMapper;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        //TODO : 14 - Creo controlador de Books
        private ILibraryRepository _libraryRepository;

        public BooksController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        //[HttpGet()]
        //public IActionResult GetBooksForAuthor(Guid authorId)
        //{
        //    //TODO : 15 -  Creo el metodo que retorne los libros de un author
        //    if (!_libraryRepository.AuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }

        //    var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

        //    var booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo);

        //    return Ok(booksForAuthor);
        //}

        [HttpGet("{id}")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            //TODO : 18 - Obtengo un libro particular
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo == null)
                return NotFound();


            var bookForAuthor = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(bookForAuthor);
       }
    }
}

using System;
using AutoMapper;
using Library.Api.Models;
using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Library.Data.Entities;
using Microsoft.AspNetCore.JsonPatch;
using Library.Api.Helper;
using Microsoft.Extensions.Logging;

namespace Library.Api.Controllers
{
    [Route("api/authors/{authorId}/books")]
    public class BooksController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly ILogger<BooksController> _logger;
        public BooksController(ILibraryRepository libraryRepository,
            ILogger<BooksController> logger)
        {
            _libraryRepository = libraryRepository;
            _logger = logger;
        }

        [HttpGet()]
        public IActionResult GetBooksForAuthor(Guid authorId)
        {
            if (!_libraryRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var booksForAuthorFromRepo = _libraryRepository.GetBooksForAuthor(authorId);

            var booksForAuthor = Mapper.Map<IEnumerable<BookDto>>(booksForAuthorFromRepo);

            return Ok(booksForAuthor);
        }

        [HttpGet("{id}", Name = "GetBookForAuthor")]
        public IActionResult GetBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo == null)
                return NotFound();


            var bookForAuthor = Mapper.Map<BookDto>(bookForAuthorFromRepo);
            return Ok(bookForAuthor);
        }

        [HttpPost()]
        public IActionResult CreateBookForAuthor(Guid authorId,
            [FromBody] BookCreationDto book)
        {
            if (book == null)
                return BadRequest();

            if (book.Description == book.Title)
                ModelState.AddModelError(nameof(BookCreationDto),
                    "El titulo debe de ser distinto a la descripción.");

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookEntity = Mapper.Map<Book>(book);

            _libraryRepository.AddBookForAuthor(authorId, bookEntity);

            if (!_libraryRepository.Save())
                throw new Exception($"La creación de libro para el autor : {authorId} no pude efectuarse.");

            var bookToReturn = Mapper.Map<BookDto>(bookEntity);

            return CreatedAtRoute("GetBookForAuthor",
                new { authorId = authorId, id = bookToReturn.Id },
                bookToReturn);
        }
        //https://github.com/aspnet/Logging
        [HttpDelete("{id}")]
        public IActionResult DeleteBookForAuthor(Guid authorId, Guid id)
        {
            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
                return NotFound();

            _libraryRepository.DeleteBook(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
                throw new Exception($"El libro {id} del autor {authorId} no pudo ser eliminado.");

            _logger.LogInformation(100, $"El libro {id} del autor {authorId} no pudo ser eliminado.");
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBookForAuthor(Guid authorId, Guid id,
          [FromBody] BookUpdateDto book)
        {
            if (book == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            if (book.Description == book.Title)
                ModelState.AddModelError(nameof(BookCreationDto),
                    "El titulo debe de ser distinto a la descripción.");

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);
            if (bookForAuthorFromRepo == null)
            {

                var bookToAdd = Mapper.Map<Book>(book);
                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                    throw new Exception($"Libro {id} del autor {authorId} fallo al intentar crearse.");

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);

                return CreatedAtRoute("GetBookForAuthor",
                    new { authorId = authorId, id = bookToReturn.Id },
                    bookToReturn);

            }

            Mapper.Map(book, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
                throw new Exception($"El libro {id} del autor {authorId} no se actualizo.");


            return NoContent();
        }




        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateBookForAuthor(Guid authorId, Guid id,
                                 [FromBody] JsonPatchDocument<BookUpdateDto> patchDoc)
        {

 
            if (patchDoc == null)
                return BadRequest();

            if (!_libraryRepository.AuthorExists(authorId))
                return NotFound();

            var bookForAuthorFromRepo = _libraryRepository.GetBookForAuthor(authorId, id);

            if (bookForAuthorFromRepo == null)
            {
                var bookDto = new BookUpdateDto();

                patchDoc.ApplyTo(bookDto, ModelState);

          

                if (bookDto.Description == bookDto.Title)
                    ModelState.AddModelError(nameof(BookCreationDto),
                        "El titulo debe de ser distinto a la descripción.");

                TryValidateModel(bookDto);

                if (!ModelState.IsValid)
                    //Si no es valido retornamos errores
                    return new UnprocessableEntityObjectResult(ModelState);



                var bookToAdd = Mapper.Map<Book>(bookDto);
                bookToAdd.Id = id;

                _libraryRepository.AddBookForAuthor(authorId, bookToAdd);

                if (!_libraryRepository.Save())
                    throw new Exception($"Error!!");

                var bookToReturn = Mapper.Map<BookDto>(bookToAdd);
                return CreatedAtRoute("GetBookForAuthor",
                    new { authorId = authorId, id = bookToReturn.Id },
                    bookToReturn);

            }

            var bookToPatch = Mapper.Map<BookUpdateDto>(bookForAuthorFromRepo);

           patchDoc.ApplyTo(bookToPatch, ModelState);

        


            TryValidateModel(bookToPatch);

            if (!ModelState.IsValid)
                return new UnprocessableEntityObjectResult(ModelState);


            //patchDoc.ApplyTo(bookToPatch);


            Mapper.Map(bookToPatch, bookForAuthorFromRepo);

            _libraryRepository.UpdateBookForAuthor(bookForAuthorFromRepo);

            if (!_libraryRepository.Save())
                throw new Exception($"Error!!");

            return NoContent();
        }
    }
}
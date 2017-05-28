using AutoMapper;
using Library.Api.Models;
using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Library.Data.Entities;
using Microsoft.AspNetCore.Http;

namespace Library.Api.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {

        private readonly ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {

            _libraryRepository = libraryRepository;
        }

        [HttpGet()]
        public IActionResult GetAuthors()
        {
            var authorsFromRepo = _libraryRepository.GetAuthors();

            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
            return Ok(authors);
        }
        [HttpGet("{id}", Name = "GetAuthor")]
        public IActionResult GetAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);

            if (authorFromRepo == null)
            {
                return NotFound();
            }
            var author = Mapper.Map<AuthorDto>(authorFromRepo);
            return Ok(author);
        }

        [HttpPost]
        public IActionResult CreateAuthor([FromBody] AuthorCreationDto author)
        {
            if (author == null)
                return BadRequest();


            var authorEntity = Mapper.Map<Author>(author);

            _libraryRepository.AddAuthor(authorEntity);

            if (!_libraryRepository.Save())
                throw new Exception("Error al crear un nuevo Author");


            var authorToReturn = Mapper.Map<AuthorDto>(authorEntity);

            return CreatedAtRoute("GetAuthor",
                new { id = authorToReturn.Id },
                authorToReturn);
        }


        [HttpPost("{id}")]
        public IActionResult BlockAuthorCreation(Guid id)
        {
            if (_libraryRepository.AuthorExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status409Conflict);
            }

            return NotFound();
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteAuthor(Guid id)
        {
            var authorFromRepo = _libraryRepository.GetAuthor(id);
            if (authorFromRepo == null)
                return NotFound();

            _libraryRepository.DeleteAuthor(authorFromRepo);

            if (!_libraryRepository.Save())
                throw new Exception($"No se pudo eliminar el uutor {id}");

            return NoContent();
        }
    }
}

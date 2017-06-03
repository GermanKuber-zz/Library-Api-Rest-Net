using AutoMapper;
using Library.Api.Models;
using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Library.Data.Entities;
using Microsoft.AspNetCore.Http;
using Library.Api.Helper;

namespace Library.Api.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {

        private readonly ILibraryRepository _libraryRepository;
        private readonly IUrlHelper _urlHelper;
        //TODO : 02 - Declaro una constante que me indíca el  tamaño máximo de la página
        //private const int pageSizeMax =10;

        public AuthorsController(ILibraryRepository libraryRepository,
            IUrlHelper urlHelper)
        {
            _libraryRepository = libraryRepository;
            //TODO : 11 - Inyecto el Servicio de Helper
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetAuthors")]
        ////TODO : 01 - Agrego parametros para la paginación
        //public IActionResult GetAuthors([FromQuery]int pageNumber = 1, [FromQuery] int pageSize= 10)
        //TODO : 04 - Recibo mi Wrapper como parametro
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            ////Agrego una validación
            //pageSize = (pageSize > pageSizeMax) ? pageSizeMax : pageSize;

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParameters);

            //TODO : 13 - Utilizamos el metodo helpe y generamos las url
            var previousPageLink = authorsFromRepo.HasPrevious ?
              CreateAuthorsResourceUri(authorsResourceParameters,
              ResourceUriType.PreviousPage) : null;

            var nextPageLink = authorsFromRepo.HasNext ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.NextPage) : null;

            //generamos la metada que voy a retornarle al usuario.
            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            //Agrego un header de paginación custom.
            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
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
        //TODO : 12 - Creo un metodo helper que me genra las url's
        private string CreateAuthorsResourceUri(
          AuthorsResourceParameters authorsResourceParameters,
          ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return _urlHelper.Link("GetAuthors",
                      new
                      {
                          //TODO : 15 - Agrego a la uri el parametro se search y genere
                          searchQuery = authorsResourceParameters.SearchQuery,
                          genre = authorsResourceParameters.Genre,

                          pageNumber = authorsResourceParameters.PageNumber - 1,
                          pageSize = authorsResourceParameters.PageSize
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                      new
                      {
                          searchQuery = authorsResourceParameters.SearchQuery,
                          genre = authorsResourceParameters.Genre,

                          pageNumber = authorsResourceParameters.PageNumber + 1,
                          pageSize = authorsResourceParameters.PageSize
                      });

                default:
                    return _urlHelper.Link("GetAuthors",
                    new
                    {
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,

                        pageNumber = authorsResourceParameters.PageNumber,
                        pageSize = authorsResourceParameters.PageSize
                    });
            }
        }
    }
    public enum ResourceUriType
    {
        PreviousPage,
        NextPage
    }
}

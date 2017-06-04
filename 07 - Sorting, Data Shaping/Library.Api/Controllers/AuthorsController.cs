using AutoMapper;
using Library.Api.Models;
using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Library.Data.Entities;
using Microsoft.AspNetCore.Http;
using Library.Api.Helper;
using Library.Data.Services;

namespace Library.Api.Controllers
{
    [Route("api/authors")]
    public class AuthorsController : Controller
    {
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingService _propertyMappingService;
        private readonly ITypeHelperService _typeHelperService;

        public AuthorsController(ILibraryRepository libraryRepository,
            IUrlHelper urlHelper
              //TODO : 12 - Inyecto el servicio para validar siempre la cadena OrderBy
              , IPropertyMappingService propertyMappingService
            //TODO : 21 - Inyecto el servicio de validación de field
            ,ITypeHelperService typeHelperService
            )
        {
            _libraryRepository = libraryRepository;
            _urlHelper = urlHelper;
            _propertyMappingService = propertyMappingService;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetAuthors")]
        public IActionResult GetAuthors(AuthorsResourceParameters authorsResourceParameters)
        {
            //TODO : 13 - Valido la entrada y en caso de error retorno 400
            if (!_propertyMappingService.ValidMappingExistsFor<AuthorDto, Author>
              (authorsResourceParameters.OrderBy))
            {
                return BadRequest();
            }

            //TODO : 20 - Valido si todas las propiedades que se solicitan en la propiedad field, correstaponden con propiedades rales del objeto
            if (!_typeHelperService.TypeHasProperties<AuthorDto>
                (authorsResourceParameters.Fields))
            {
                return BadRequest();
            }

            var authorsFromRepo = _libraryRepository.GetAuthors(authorsResourceParameters);
            var previousPageLink = authorsFromRepo.HasPrevious ?
              CreateAuthorsResourceUri(authorsResourceParameters,
              ResourceUriType.PreviousPage) : null;

            var nextPageLink = authorsFromRepo.HasNext ?
                CreateAuthorsResourceUri(authorsResourceParameters,
                ResourceUriType.NextPage) : null;

            var paginationMetadata = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousPageLink = previousPageLink,
                nextPageLink = nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
            var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);


            //return Ok(authors);

            //TODO : 16 - Se utiliza el método de extensión
            return Ok(authors.ShapeData(authorsResourceParameters.Fields));

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
                          searchQuery = authorsResourceParameters.SearchQuery,
                          genre = authorsResourceParameters.Genre,
                          pageNumber = authorsResourceParameters.PageNumber - 1,
                          pageSize = authorsResourceParameters.PageSize,
                          //TODO : 10 - Agrego campos de orden y de filtrado a las respuestas.
                          orderBy = authorsResourceParameters.OrderBy,
                          //TODO : 22 -  Retorno la propiedad de field en los links
                          fields = authorsResourceParameters.Fields
                    
                      });
                case ResourceUriType.NextPage:
                    return _urlHelper.Link("GetAuthors",
                      new
                      {

                          searchQuery = authorsResourceParameters.SearchQuery,
                          genre = authorsResourceParameters.Genre,
                          pageNumber = authorsResourceParameters.PageNumber + 1,
                          pageSize = authorsResourceParameters.PageSize,

                          fields = authorsResourceParameters.Fields,
                          orderBy = authorsResourceParameters.OrderBy
                      });

                default:
                    return _urlHelper.Link("GetAuthors",
                    new
                    {
                        searchQuery = authorsResourceParameters.SearchQuery,
                        genre = authorsResourceParameters.Genre,
                        pageNumber = authorsResourceParameters.PageNumber,
                        pageSize = authorsResourceParameters.PageSize,

                        fields = authorsResourceParameters.Fields,
                        orderBy = authorsResourceParameters.OrderBy
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

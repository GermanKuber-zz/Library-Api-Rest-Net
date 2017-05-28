using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Library.Api.Binders;
using Library.Api.Models;
using Library.Data.Entities;
using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Library.API.Controllers
{
    [Route("api/authorcollections")]
    public class AuthorCollectionsController : Controller
    {
        //Controlador para administrar colecciones de autores
        private readonly ILibraryRepository _libraryRepository;

        public AuthorCollectionsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }

        
        [HttpPost]
        public IActionResult CreateAuthorCollection(
            [FromBody] IEnumerable<AuthorCreationDto> authorCollection)
        {
            if (authorCollection == null)
                return BadRequest();

            var authorEntities = Mapper.Map<IEnumerable<Author>>(authorCollection);

            foreach (var author in authorEntities)
                _libraryRepository.AddAuthor(author);

            if (!_libraryRepository.Save())
            {
                throw new Exception("Error al crear una colección de Autores");
            }

            var authorCollectionToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            var idsAsString = string.Join(",",
                authorCollectionToReturn.Select(a => a.Id));

            return CreatedAtRoute("GetAuthorCollection",
                new { ids = idsAsString },
                authorCollectionToReturn);
        }

        // (key1,key2, ...)

        [HttpGet("({ids})", Name="GetAuthorCollection")]
        public IActionResult GetAuthorCollection(
            [ModelBinder(BinderType = typeof(ListModelBinder))] IEnumerable<Guid> ids)
        {         
            if (ids == null)
                return BadRequest();

            var authorEntities = _libraryRepository.GetAuthors(ids);

            if (ids.Count() != authorEntities.Count())
                return NotFound();


            var authorsToReturn = Mapper.Map<IEnumerable<AuthorDto>>(authorEntities);
            return Ok(authorsToReturn);
        }
    }
}

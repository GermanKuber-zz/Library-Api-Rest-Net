using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
    //TODO : 05 - Implemento routing a nivel de controller
    //[Route("api/authors")]
    //[Route("api/[controller]")]
    public class AuthorsController : Controller
    {
        //TODO : 01 - Creo un controller de Authors
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            //TODO : 02 - Inyecto la dependencia al repositorio
            _libraryRepository = libraryRepository;
        }
        //TODO : 04 - Implemento el routing
        //[HttpGet("api/authors")]
        public IActionResult GetAuthors()
        {
            //TODO : 03 - Implemento el metodo de GetAuthors y retorno un JSON
            var authorsFromRepo = _libraryRepository.GetAuthors();

            return new JsonResult(authorsFromRepo);
        }


        //[HttpGet()]
        //public IActionResult GetAuthors()
        //{
        //    //TODO : 07 - Retorno DTO en lugar  de objetos de negocio/EF
        //    var authorsFromRepo = _libraryRepository.GetAuthors();

        //    var returnAuthors = new List<AuthorDto>();

        //    foreach (var item in authorsFromRepo)
        //    {
        //        returnAuthors.Add(new AuthorDto
        //        {
        //            Id = item.Id,
        //            Name = $"{item.FirstName} - {item.LastName}",
        //            Genre = item.Genre,
        //            Age = item.DateOfBirth.GetCurrentAge()
        //        });

        //    }

        //    return new JsonResult(returnAuthors);
        //}
        //[HttpGet()]
        //public IActionResult GetAuthors()
        //{
        //    //TODO : 09 - Implemento metodo con automapper
        //    var authorsFromRepo = _libraryRepository.GetAuthors();

        //    var authors = Mapper.Map<IEnumerable<AuthorDto>>(authorsFromRepo);
        //    return Ok(authors);
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetAuthor(Guid id)
        //{
        //    //TODO : 10 - Implemento metodo para obtener un solo recurso

        //    var authorFromRepo = _libraryRepository.GetAuthor(id);

        //    //if (authorFromRepo == null)
        //    //{
        //    //    //TODO : 11 - Retorno código 404 NotFound
        //    //    return NotFound();
        //    //}
        //    var author = Mapper.Map<AuthorDto>(authorFromRepo);
        //    return Ok(author);
        //}


        //[HttpGet("exception")]
        //public IActionResult GetAuthorWithException()
        //{
        //    //TODO : 12 - implemento metodo que retornar una exception

        //    //throw new Exception("Error!!!!");

        //    ////try
        //    ////{
        //    ////    throw new Exception("Error!!!!");
        //    ////}
        //    ////catch (Exception ex)
        //    ////{
        //    ////    return StatusCode(500, "Ocurrio un error. Intente nuevamente o ponganse en contacto con admin@library.com");
        //    ////}
        //    return Ok();
        //}
    }
}

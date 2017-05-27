using Library.Data.Respositories;
using Microsoft.AspNetCore.Mvc;

namespace Library.Api.Controllers
{
   
    public class AuthorsController : Controller
    {
       
        private ILibraryRepository _libraryRepository;

        public AuthorsController(ILibraryRepository libraryRepository)
        {
            _libraryRepository = libraryRepository;
        }
        
    }
}

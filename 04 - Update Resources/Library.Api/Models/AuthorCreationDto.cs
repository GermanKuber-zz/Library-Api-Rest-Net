using System;
using System.Collections.Generic;

namespace Library.Api.Models
{
    public class AuthorCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }

        //Se agrega con la intención de poder crear un Author y sus propios libros de una
        public ICollection<BookCreationDto> Books { get; set; }
            = new List<BookCreationDto>();
    }
}
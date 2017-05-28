using System;
using System.Collections.Generic;

namespace Library.Api.Models
{
    //TODO : 03 - Agrego clases DTO para crear

    public class AuthorCreationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string Genre { get; set; }

        public ICollection<BookCreationDto> Books { get; set; }
            = new List<BookCreationDto>();
    }
}
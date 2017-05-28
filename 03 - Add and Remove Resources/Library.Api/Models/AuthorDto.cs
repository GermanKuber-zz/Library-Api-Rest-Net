using System;

namespace Library.Api.Models
{
    //TODO : 06 - Creo DTO para retornar
    public class AuthorDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Genre { get; set; }
    }
}

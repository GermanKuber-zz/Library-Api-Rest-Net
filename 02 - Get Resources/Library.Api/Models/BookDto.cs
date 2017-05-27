using System;

namespace Library.Api.Models
{
    //TODO : 16 -  Creo un DTO para books
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid AuthorId { get; set; }
    }
}

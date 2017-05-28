using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models
{
    //TODO : 02 - Creo clase base para ser utilizada por los book a ser modificados
    public abstract class BookChangesDto
    {
        public string Title { get; set; }
        public virtual string Description { get; set; }
    }
}

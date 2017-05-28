using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models
{
    //TODO : 01 - Agrego validaciones y mensajes de error
    public abstract class BookChangesDto
    {
        [Required(ErrorMessage = "Por favor ingrese una descipción.")]
        [MaxLength(100, ErrorMessage = "El titulo no debe tener mas de 100 caracteres.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "El titulo no debe tener mas de 500 caracteres")]
        public virtual string Description { get; set; }
    }
}

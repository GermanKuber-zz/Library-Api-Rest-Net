using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models
{
    //TODO : 02 - Agrego validaciones y mensajes de error
    public class BookUpdateDto : BookChangesDto
    {
        [Required(ErrorMessage = "Por favor ingrese una descipción.")]
        public override string Description
        {
            get
            {
                return base.Description;
            }

            set
            {
                base.Description = value;
            }
        }
    }
}

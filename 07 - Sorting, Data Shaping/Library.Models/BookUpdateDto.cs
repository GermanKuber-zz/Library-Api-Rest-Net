using System.ComponentModel.DataAnnotations;

namespace Library.Api.Models
{
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

using System.ComponentModel.DataAnnotations;

namespace TSA.Infrastructure.DTOs
{
    public class PolicyTypeDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Descripción es requerido")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }
        public bool IsActive { get; set; }

    }
}

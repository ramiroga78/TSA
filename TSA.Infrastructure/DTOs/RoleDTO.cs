using System.ComponentModel.DataAnnotations;

namespace TSA.Infrastructure.DTOs
{
    public class RoleDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo Nombre es requerido")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        public bool IsActive { get; set; }

    }
}

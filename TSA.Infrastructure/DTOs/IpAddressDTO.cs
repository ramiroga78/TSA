using System.ComponentModel.DataAnnotations;
using TSA.Infrastructure.Helpers;

namespace TSA.Infrastructure.DTOs
{
    public class IpAddressDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo IP Desde es requerido")]
        [IpValidator(ErrorMessage = "El campo IP Desde tiene formato incorrecto")]    
        [Display(Name = "IP Desde")]
        public string Name { get; set; }
        [Required(ErrorMessage = "El campo IP Hasta es requerido")]
        [IpValidator(ErrorMessage = "El campo IP Hasta tiene formato incorrecto")]
        [Display(Name = "IP Hasta")]
        public string Ip { get; set; }
        public bool IsActive { get; set; }
    }
}

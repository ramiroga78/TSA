using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSALibrary.Models;

namespace TSA.Infrastructure.DTOs
{
    public class CertificateOrganizationDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo CommonName es requerido")]
        [Display(Name = "Common Name")]
        public string CommonName { get; set; }
        [Display(Name = "Nombre de la Organización")]
        public string OrganizationName { get; set; }
        public string CountryName { get; set; }
        public DateTime AddDate { get; set; }
        public bool IsActive { get; set; }
        //[Required(ErrorMessage = "El campo CommonName es requerido")]
        //[Range(1,2, ErrorMessage = "Debe seleccionar un Tipo")]
        [Display(Name = "Tipo de Organización")]
        public int CertificateOrganizationTypeId { get; set; }
        [Display(Name = "País")]
        public List<String> CountryList { get; set; }
        public CertificateOrganizationType CertificateOrganizationType { get; set; }
        public List<CertificateOrganizationType> CertificateOrganizationTypes{ get; set; }
    }
}

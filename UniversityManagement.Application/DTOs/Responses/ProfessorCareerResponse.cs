using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.DTOs.Responses
{
    public class ProfessorCareerResponse
    {
        public int CarreraId { get; set; }
        public string NombreCarrera { get; set; } = string.Empty;
        public string? DescripcionCarrera { get; set; }
        public string? FacultadNombre { get; set; }
        public DateTime FechaAsignacion { get; set; }
        public bool Activo { get; set; }
    }
}

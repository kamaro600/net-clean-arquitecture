using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Infrastructure.Data.Common;

namespace UniversityManagement.Infrastructure.Data.Models
{
    /// <summary>
    /// Modelo de datos para Profesor
    /// </summary>
    public class ProfessorDataModel : BaseDataModel
    {
        public int ProfessorId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Dni { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Specialty { get; set; }
        public string? AcademicDegree { get; set; }

        // Propiedades de navegación
        public virtual ICollection<ProfessorCareerDataModel> ProfessorCareers { get; set; } = new List<ProfessorCareerDataModel>();
    }
}

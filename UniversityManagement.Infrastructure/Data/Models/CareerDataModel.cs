using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Infrastructure.Data.Common;

namespace UniversityManagement.Infrastructure.Data.Models
{
    /// <summary>
    /// Modelo de datos para Carrera
    /// </summary>
    public class CareerDataModel : BaseDataModel
    {
        public int CareerId { get; set; }
        public int FacultyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SemesterDuration { get; set; }
        public string? AwardedTitle { get; set; }

        // Propiedades de navegación
        public virtual FacultyDataModel Faculty { get; set; } = null!;
        public virtual ICollection<StudentCareerDataModel> StudentCareers { get; set; } = new List<StudentCareerDataModel>();
        public virtual ICollection<ProfessorCareerDataModel> ProfessorCareers { get; set; } = new List<ProfessorCareerDataModel>();
    }
}

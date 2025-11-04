using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Infrastructure.Data.Models
{
    /// <summary>
    /// Modelo de datos para la relación N:M Profesor-Carrera
    /// </summary>
    public class ProfessorCareerDataModel
    {
        public int ProfessorId { get; set; }
        public int CareerId { get; set; }
        public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public virtual ProfessorDataModel Professor { get; set; } = null!;
        public virtual CareerDataModel Career { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Infrastructure.Data.Models
{

    /// <summary>
    /// Modelo de datos para la relación N:M Estudiante-Carrera
    /// </summary>
    public class StudentCareerDataModel
    {
        public int StudentId { get; set; }
        public int CareerId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Propiedades de navegación
        public virtual StudentDataModel Student { get; set; } = null!;
        public virtual CareerDataModel Career { get; set; } = null!;
    }
}

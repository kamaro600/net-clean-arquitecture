using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Infrastructure.Data.Common;

namespace UniversityManagement.Infrastructure.Data.Models
{
    /// <summary>
    /// Modelo de datos para Facultad
    /// </summary>
    public class FacultyDataModel : BaseDataModel
    {
        public int FacultyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? Dean { get; set; }

        // Propiedades de navegación
        public virtual ICollection<CareerDataModel> Careers { get; set; } = new List<CareerDataModel>();
    }
}

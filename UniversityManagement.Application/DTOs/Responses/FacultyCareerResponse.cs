using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.DTOs.Responses
{
    public class FacultyCareerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int SemesterDuration { get; set; }
        public bool IsActive { get; set; }
    }
}

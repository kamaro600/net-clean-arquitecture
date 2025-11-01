using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.DTOs.Responses
{
    public class StudentCareerResponse
    {
        public int CareerId { get; set; }
        public string CareerName { get; set; } = string.Empty;
        public string? CareerDescription { get; set; }
        public string? FacultyName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public bool IsActive { get; set; }
    }
}

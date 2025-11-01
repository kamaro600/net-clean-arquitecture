using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.DTOs.Queries
{
    public class GetStudentByIdQuery
    {
        public int StudentId { get; set; }

        public GetStudentByIdQuery(int studentId)
        {
            StudentId = studentId;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.DTOs.Queries
{
    public class GetStudentByDniQuery
    {
        public string Dni { get; set; } = string.Empty;

        public GetStudentByDniQuery(string dni)
        {
            Dni = dni;
        }
    }
}

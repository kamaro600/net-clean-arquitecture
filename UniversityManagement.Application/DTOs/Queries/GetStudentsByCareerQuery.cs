using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.DTOs.Queries
{
    public class GetStudentsByCareerQuery
    {
        public int CareerId { get; set; }

        public GetStudentsByCareerQuery(int careerId)
        {
            CareerId = careerId;
        }
    }
}

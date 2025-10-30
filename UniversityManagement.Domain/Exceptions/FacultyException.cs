using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Domain.Exceptions
{
    public class FacultyNotFoundException : DomainException
    {
        public FacultyNotFoundException(int facultyId)
            : base($"La facultad con ID {facultyId} no fue encontrado.")
        {
        }

    }

    public class DuplicateFacultyException : DomainException
    {
        public DuplicateFacultyException(string field, string value)
            : base($"Ya existe una facultad con {field}: {value}")
        {
        }
    }
}

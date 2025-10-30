using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Domain.Exceptions
{
    public class CareerNotFoundException : DomainException
    {
        public CareerNotFoundException(int careerId)
            : base($"La carrera con ID {careerId} no fue encontrado.")
        {
        }

    }

    public class DuplicateCareerException : DomainException
    {
        public DuplicateCareerException(string field, string value)
            : base($"Ya existe una carrera con {field}: {value}")
        {
        }
    }
}

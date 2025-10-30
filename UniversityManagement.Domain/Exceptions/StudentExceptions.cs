namespace UniversityManagement.Domain.Exceptions;


public class StudentNotFoundException : DomainException
{
    public StudentNotFoundException(int studentId) 
        : base($"El estudiante con ID {studentId} no fue encontrado.")
    {
    }

    public StudentNotFoundException(string dni) 
        : base($"El estudiante con DNI {dni} no fue encontrado.")
    {
    }
}

public class DuplicateStudentException : DomainException
{
    public DuplicateStudentException(string field, string value) 
        : base($"Ya existe un estudiante con {field}: {value}")
    {
    }
}

public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string email) 
        : base($"El email '{email}' no tiene un formato válido.")
    {
    }
}

public class InvalidDniException : DomainException
{
    public InvalidDniException(string dni) 
        : base($"El DNI '{dni}' no tiene un formato válido.")
    {
    }
}

public class StudentTooYoungException : DomainException
{
    public StudentTooYoungException(DateTime birthDate) 
        : base($"El estudiante debe tener al menos 16 años. Fecha de nacimiento: {birthDate:dd/MM/yyyy}")
    {
    }
}
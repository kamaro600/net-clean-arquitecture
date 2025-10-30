namespace UniversityManagement.Domain.Exceptions;

public class ProfessorNotFoundException : DomainException
{
    public ProfessorNotFoundException(int professorId) 
        : base($"El profesor con ID {professorId} no fue encontrado.")
    {
    }

    public ProfessorNotFoundException(string dni) 
        : base($"El profesor con DNI {dni} no fue encontrado.")
    {
    }
}

public class DuplicateProfessorException : DomainException
{
    public DuplicateProfessorException(string field, string value) 
        : base($"Ya existe un profesor con {field}: {value}")
    {
    }
}

public class ProfessorAlreadyAssignedException : DomainException
{
    public ProfessorAlreadyAssignedException(int professorId, int careerId) 
        : base($"El profesor con ID {professorId} ya está asignado a la carrera con ID {careerId}")
    {
    }
}
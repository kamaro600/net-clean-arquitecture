using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para conversión entre ProfessorDomain (dominio) y ProfessorDataModel (persistencia)
/// </summary>
public static class ProfessorMapper
{
    /// <summary>
    /// Convierte de modelo de datos (EF Core) a entidad de dominio
    /// </summary>
    public static Professor ToDomain(ProfessorDataModel dataModel)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));

        try
        {
            return new Professor(
                professorId: dataModel.ProfessorId,
                firstName: dataModel.FirstName,
                lastName: dataModel.LastName,
                dni: dataModel.Dni,
                email: dataModel.Email,
                phone: dataModel.Phone,
                specialty: dataModel.Specialty,
                academicDegree: dataModel.AcademicDegree,
                fechaRegistro: dataModel.FechaRegistro,
                activo: dataModel.Activo
            );
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            throw new InvalidOperationException(
                $"Error al mapear ProfessorDataModel a ProfessorDomain para ID {dataModel.ProfessorId}: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Convierte de entidad de dominio a modelo de datos (EF Core)
    /// </summary>
    public static ProfessorDataModel ToDataModel(Professor domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new ProfessorDataModel
        {
            ProfessorId = domain.ProfessorId,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Dni = domain.Dni,
            Email = domain.Email,
            Phone = domain.Phone,
            Specialty = domain.Specialty,
            AcademicDegree = domain.AcademicDegree,
            FechaRegistro = domain.FechaRegistro,
            Activo = domain.Activo
        };
    }

    /// <summary>
    /// Convierte una colección de modelos de datos a entidades de dominio
    /// </summary>
    public static IEnumerable<Professor> ToDomain(IEnumerable<ProfessorDataModel> dataModels)
    {
        if (dataModels == null)
            return Enumerable.Empty<Professor>();

        var result = new List<Professor>();
        foreach (var dataModel in dataModels)
        {
            try
            {
                result.Add(ToDomain(dataModel));
            }
            catch (InvalidOperationException)
            {
                continue;
            }
        }
        return result;
    }

    /// <summary>
    /// Convierte una colección de entidades de dominio a modelos de datos
    /// </summary>
    public static IEnumerable<ProfessorDataModel> ToDataModel(IEnumerable<Professor> domains)
    {
        if (domains == null)
            return Enumerable.Empty<ProfessorDataModel>();

        return domains.Select(ToDataModel);
    }

    /// <summary>
    /// Actualiza un modelo de datos existente con los valores de una entidad de dominio
    /// </summary>
    public static void UpdateDataModelFromDomain(ProfessorDataModel dataModel, Professor domain)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        dataModel.FirstName = domain.FirstName;
        dataModel.LastName = domain.LastName;
        dataModel.Dni = domain.Dni;
        dataModel.Email = domain.Email;
        dataModel.Phone = domain.Phone;
        dataModel.Specialty = domain.Specialty;
        dataModel.AcademicDegree = domain.AcademicDegree;
        dataModel.Activo = domain.Activo;
    }

    /// <summary>
    /// Verifica si un modelo de datos tiene valores válidos para convertir a dominio
    /// </summary>
    public static bool IsValidForDomainConversion(ProfessorDataModel dataModel)
    {
        if (dataModel == null)
            return false;

        return !string.IsNullOrWhiteSpace(dataModel.FirstName) &&
               !string.IsNullOrWhiteSpace(dataModel.LastName) &&
               !string.IsNullOrWhiteSpace(dataModel.Dni) &&
               !string.IsNullOrWhiteSpace(dataModel.Email);
    }
}
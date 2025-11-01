using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para conversión entre StudentCareerDomain (dominio) y StudentCareerDataModel (persistencia)
/// </summary>
public static class StudentCareerMapper
{
    /// <summary>
    /// Convierte de modelo de datos (EF Core) a entidad de dominio
    /// </summary>
    public static StudentCareer ToDomain(StudentCareerDataModel dataModel)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));

        try
        {
            return new StudentCareer(
                studentId: dataModel.StudentId,
                careerId: dataModel.CareerId,
                enrollmentDate: dataModel.EnrollmentDate,
                isActive: dataModel.IsActive
            );
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            throw new InvalidOperationException(
                $"Error al mapear StudentCareerDataModel a StudentCareerDomain para StudentId {dataModel.StudentId}, CareerId {dataModel.CareerId}: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Convierte de entidad de dominio a modelo de datos (EF Core)
    /// </summary>
    public static StudentCareerDataModel ToDataModel(StudentCareer domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new StudentCareerDataModel
        {
            StudentId = domain.StudentId,
            CareerId = domain.CareerId,
            EnrollmentDate = domain.EnrollmentDate,
            IsActive = domain.IsActive
        };
    }

    /// <summary>
    /// Convierte una colección de modelos de datos a entidades de dominio
    /// </summary>
    public static IEnumerable<StudentCareer> ToDomain(IEnumerable<StudentCareerDataModel> dataModels)
    {
        if (dataModels == null)
            return Enumerable.Empty<StudentCareer>();

        var result = new List<StudentCareer>();
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
    public static IEnumerable<StudentCareerDataModel> ToDataModel(IEnumerable<StudentCareer> domains)
    {
        if (domains == null)
            return Enumerable.Empty<StudentCareerDataModel>();

        return domains.Select(ToDataModel);
    }

    /// <summary>
    /// Actualiza un modelo de datos existente con los valores de una entidad de dominio
    /// </summary>
    public static void UpdateDataModelFromDomain(StudentCareerDataModel dataModel, StudentCareer domain)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        // Los IDs no se actualizan ya que son parte de la clave primaria compuesta
        dataModel.EnrollmentDate = domain.EnrollmentDate;
        dataModel.IsActive = domain.IsActive;
    }

    /// <summary>
    /// Verifica si un modelo de datos tiene valores válidos para convertir a dominio
    /// </summary>
    public static bool IsValidForDomainConversion(StudentCareerDataModel dataModel)
    {
        if (dataModel == null)
            return false;

        return dataModel.StudentId > 0 && dataModel.CareerId > 0;
    }
}
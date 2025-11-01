using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para conversión entre ProfessorCareerDomain (dominio) y ProfessorCareerDataModel (persistencia)
/// </summary>
public static class ProfessorCareerMapper
{
    /// <summary>
    /// Convierte de modelo de datos (EF Core) a entidad de dominio
    /// </summary>
    public static ProfessorCareer ToDomain(ProfessorCareerDataModel dataModel)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));

        try
        {
            return new ProfessorCareer(
                professorId: dataModel.ProfessorId,
                careerId: dataModel.CareerId,
                assignmentDate: dataModel.AssignmentDate,
                isActive: dataModel.IsActive
            );
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            throw new InvalidOperationException(
                $"Error al mapear ProfessorCareerDataModel a ProfessorCareerDomain para ProfessorId {dataModel.ProfessorId}, CareerId {dataModel.CareerId}: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Convierte de entidad de dominio a modelo de datos (EF Core)
    /// </summary>
    public static ProfessorCareerDataModel ToDataModel(ProfessorCareer domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new ProfessorCareerDataModel
        {
            ProfessorId = domain.ProfessorId,
            CareerId = domain.CareerId,
            AssignmentDate = domain.AssignmentDate,
            IsActive = domain.IsActive
        };
    }

    /// <summary>
    /// Convierte una colección de modelos de datos a entidades de dominio
    /// </summary>
    public static IEnumerable<ProfessorCareer> ToDomain(IEnumerable<ProfessorCareerDataModel> dataModels)
    {
        if (dataModels == null)
            return Enumerable.Empty<ProfessorCareer>();

        var result = new List<ProfessorCareer>();
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
    public static IEnumerable<ProfessorCareerDataModel> ToDataModel(IEnumerable<ProfessorCareer> domains)
    {
        if (domains == null)
            return Enumerable.Empty<ProfessorCareerDataModel>();

        return domains.Select(ToDataModel);
    }

    /// <summary>
    /// Actualiza un modelo de datos existente con los valores de una entidad de dominio
    /// </summary>
    public static void UpdateDataModelFromDomain(ProfessorCareerDataModel dataModel, ProfessorCareer domain)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        // Los IDs no se actualizan ya que son parte de la clave primaria compuesta
        dataModel.AssignmentDate = domain.AssignmentDate;
        dataModel.IsActive = domain.IsActive;
    }

    /// <summary>
    /// Verifica si un modelo de datos tiene valores válidos para convertir a dominio
    /// </summary>
    public static bool IsValidForDomainConversion(ProfessorCareerDataModel dataModel)
    {
        if (dataModel == null)
            return false;

        return dataModel.ProfessorId > 0 && dataModel.CareerId > 0;
    }
}
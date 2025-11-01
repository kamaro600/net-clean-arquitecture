using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para conversión entre FacultyDomain (dominio) y FacultyDataModel (persistencia)
/// </summary>
public static class FacultyMapper
{
    /// <summary>
    /// Convierte de modelo de datos (EF Core) a entidad de dominio
    /// </summary>
    public static Faculty ToDomain(FacultyDataModel dataModel)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));

        try
        {
            return new Faculty(
                facultyId: dataModel.FacultyId,
                name: dataModel.Name,
                description: dataModel.Description,
                location: dataModel.Location,
                dean: dataModel.Dean,
                fechaRegistro: dataModel.FechaRegistro,
                activo: dataModel.Activo
            );
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            throw new InvalidOperationException(
                $"Error al mapear FacultyDataModel a FacultyDomain para ID {dataModel.FacultyId}: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Convierte de entidad de dominio a modelo de datos (EF Core)
    /// </summary>
    public static FacultyDataModel ToDataModel(Faculty domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new FacultyDataModel
        {
            FacultyId = domain.FacultyId,
            Name = domain.Name,
            Description = domain.Description,
            Location = domain.Location,
            Dean = domain.Dean,
            FechaRegistro = domain.FechaRegistro,
            Activo = domain.Activo
        };
    }

    /// <summary>
    /// Convierte una colección de modelos de datos a entidades de dominio
    /// </summary>
    public static IEnumerable<Faculty> ToDomain(IEnumerable<FacultyDataModel> dataModels)
    {
        if (dataModels == null)
            return Enumerable.Empty<Faculty>();

        var result = new List<Faculty>();
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
    public static IEnumerable<FacultyDataModel> ToDataModel(IEnumerable<Faculty> domains)
    {
        if (domains == null)
            return Enumerable.Empty<FacultyDataModel>();

        return domains.Select(ToDataModel);
    }

    /// <summary>
    /// Actualiza un modelo de datos existente con los valores de una entidad de dominio
    /// </summary>
    public static void UpdateDataModelFromDomain(FacultyDataModel dataModel, Faculty domain)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        dataModel.Name = domain.Name;
        dataModel.Description = domain.Description;
        dataModel.Location = domain.Location;
        dataModel.Dean = domain.Dean;
        dataModel.Activo = domain.Activo;
    }

    /// <summary>
    /// Verifica si un modelo de datos tiene valores válidos para convertir a dominio
    /// </summary>
    public static bool IsValidForDomainConversion(FacultyDataModel dataModel)
    {
        if (dataModel == null)
            return false;

        return !string.IsNullOrWhiteSpace(dataModel.Name);
    }
}
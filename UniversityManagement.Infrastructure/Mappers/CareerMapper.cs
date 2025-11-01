using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para conversión entre CareerDomain (dominio) y CareerDataModel (persistencia)
/// </summary>
public static class CareerMapper
{
    /// <summary>
    /// Convierte de modelo de datos (EF Core) a entidad de dominio
    /// </summary>
    public static Career ToDomain(CareerDataModel dataModel)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));

        try
        {
            return new Career(
                careerId: dataModel.CareerId,
                facultyId: dataModel.FacultyId,
                name: dataModel.Name,
                semesterDuration: dataModel.SemesterDuration,
                description: dataModel.Description,
                awardedTitle: dataModel.AwardedTitle,
                fechaRegistro: dataModel.FechaRegistro,
                activo: dataModel.Activo
            );
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            throw new InvalidOperationException(
                $"Error al mapear CareerDataModel a CareerDomain para ID {dataModel.CareerId}: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Convierte de entidad de dominio a modelo de datos (EF Core)
    /// </summary>
    public static CareerDataModel ToDataModel(Career domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new CareerDataModel
        {
            CareerId = domain.CareerId,
            FacultyId = domain.FacultyId,
            Name = domain.Name,
            Description = domain.Description,
            SemesterDuration = domain.SemesterDuration,
            AwardedTitle = domain.AwardedTitle,
            FechaRegistro = domain.FechaRegistro,
            Activo = domain.Activo
        };
    }

    /// <summary>
    /// Convierte una colección de modelos de datos a entidades de dominio
    /// </summary>
    public static IEnumerable<Career> ToDomain(IEnumerable<CareerDataModel> dataModels)
    {
        if (dataModels == null)
            return Enumerable.Empty<Career>();

        var result = new List<Career>();
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
    public static IEnumerable<CareerDataModel> ToDataModel(IEnumerable<Career> domains)
    {
        if (domains == null)
            return Enumerable.Empty<CareerDataModel>();

        return domains.Select(ToDataModel);
    }

    /// <summary>
    /// Actualiza un modelo de datos existente con los valores de una entidad de dominio
    /// </summary>
    public static void UpdateDataModelFromDomain(CareerDataModel dataModel, Career domain)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        dataModel.FacultyId = domain.FacultyId;
        dataModel.Name = domain.Name;
        dataModel.Description = domain.Description;
        dataModel.SemesterDuration = domain.SemesterDuration;
        dataModel.AwardedTitle = domain.AwardedTitle;
        dataModel.Activo = domain.Activo;
    }

    /// <summary>
    /// Verifica si un modelo de datos tiene valores válidos para convertir a dominio
    /// </summary>
    public static bool IsValidForDomainConversion(CareerDataModel dataModel)
    {
        if (dataModel == null)
            return false;

        return !string.IsNullOrWhiteSpace(dataModel.Name) && 
               dataModel.FacultyId > 0 && 
               dataModel.SemesterDuration > 0;
    }
}
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para conversión entre StudentDomain (dominio) y StudentDataModel (persistencia)
/// </summary>
public static class StudentMapper
{
    /// <summary>
    /// Convierte de modelo de datos (EF Core) a entidad de dominio
    /// </summary>
    /// <param name="dataModel">Modelo de datos desde la base de datos</param>
    /// <returns>Entidad de dominio con Value Objects</returns>
    /// <exception cref="ArgumentNullException">Si dataModel es null</exception>
    /// <exception cref="ArgumentException">Si algún Value Object es inválido</exception>
    public static Student ToDomain(StudentDataModel dataModel)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));

        try
        {
            // Crear Value Objects con validación automática
            var fullName = new FullName(dataModel.Nombre, dataModel.Apellido);
            var dni = new Dni(dataModel.Dni);
            var email = new Email(dataModel.Email);
            
            // Value Objects opcionales
            Phone? phone = null;
            if (!string.IsNullOrWhiteSpace(dataModel.Telefono))
            {
                phone = new Phone(dataModel.Telefono);
            }
            
            Address? address = null;
            if (!string.IsNullOrWhiteSpace(dataModel.Direccion))
            {
                address = new Address(dataModel.Direccion);
            }

            return new Student(
                id: dataModel.EstudianteId,
                fullName: fullName,
                dni: dni,
                email: email,
                birthdate: dataModel.FechaNacimiento,
                phone: phone,
                address: address,
                registrationDate: dataModel.FechaRegistro,
                isActive: dataModel.Activo
            );
        }
        catch (Exception ex) when (!(ex is ArgumentNullException))
        {
            // Loggear el error en un entorno real
            throw new InvalidOperationException(
                $"Error al mapear StudentDataModel a StudentDomain para ID {dataModel.EstudianteId}: {ex.Message}", 
                ex);
        }
    }

    /// <summary>
    /// Convierte de entidad de dominio a modelo de datos (EF Core)
    /// </summary>
    /// <param name="domain">Entidad de dominio</param>
    /// <returns>Modelo de datos para persistencia</returns>
    /// <exception cref="ArgumentNullException">Si domain es null</exception>
    public static StudentDataModel ToDataModel(Student domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new StudentDataModel
        {
            EstudianteId = domain.Id,
            Nombre = domain.FullName.FirstName,
            Apellido = domain.FullName.LastName,
            Dni = domain.Dni.Value,
            Email = domain.Email.Value,
            Telefono = domain.Phone?.Value,
            FechaNacimiento = domain.Birthdate,
            Direccion = domain.Address?.ToString(),
            FechaRegistro = domain.RegistrationDate,
            Activo = domain.IsActive
        };
    }

    /// <summary>
    /// Convierte una colección de modelos de datos a entidades de dominio
    /// </summary>
    /// <param name="dataModels">Colección de modelos de datos</param>
    /// <returns>Colección de entidades de dominio</returns>
    public static IEnumerable<Student> ToDomain(IEnumerable<StudentDataModel> dataModels)
    {
        if (dataModels == null)
            return Enumerable.Empty<Student>();

        var result = new List<Student>();
        foreach (var dataModel in dataModels)
        {
            try
            {
                result.Add(ToDomain(dataModel));
            }
            catch (InvalidOperationException)
            {
                // En un entorno real, loggearíamos esto y podríamos decidir si continuar o fallar
                // Por ahora, omitimos registros inválidos
                continue;
            }
        }
        return result;
    }

    /// <summary>
    /// Convierte una colección de entidades de dominio a modelos de datos
    /// </summary>
    /// <param name="domains">Colección de entidades de dominio</param>
    /// <returns>Colección de modelos de datos</returns>
    public static IEnumerable<StudentDataModel> ToDataModel(IEnumerable<Student> domains)
    {
        if (domains == null)
            return Enumerable.Empty<StudentDataModel>();

        return domains.Select(ToDataModel);
    }

    /// <summary>
    /// Actualiza un modelo de datos existente con los valores de una entidad de dominio
    /// Útil para operaciones de actualización donde queremos preservar el tracking de EF Core
    /// </summary>
    /// <param name="dataModel">Modelo de datos existente a actualizar</param>
    /// <param name="domain">Entidad de dominio con los nuevos valores</param>
    /// <exception cref="ArgumentNullException">Si algún parámetro es null</exception>
    public static void UpdateDataModelFromDomain(StudentDataModel dataModel, Student domain)
    {
        if (dataModel == null)
            throw new ArgumentNullException(nameof(dataModel));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        // Actualizar todas las propiedades excepto ID y fechas de auditoría base
        dataModel.Nombre = domain.FullName.FirstName;
        dataModel.Apellido = domain.FullName.LastName;
        dataModel.Dni = domain.Dni.Value;
        dataModel.Email = domain.Email.Value;
        dataModel.Telefono = domain.Phone?.Value;
        dataModel.FechaNacimiento = domain.Birthdate;
        dataModel.Direccion = domain.Address?.ToString();
        dataModel.Activo = domain.IsActive;
        // FechaRegistro no se actualiza - se mantiene la original
    }

    /// <summary>
    /// Verifica si un modelo de datos tiene valores válidos para convertir a dominio
    /// Útil para validación antes de la conversión
    /// </summary>
    /// <param name="dataModel">Modelo de datos a validar</param>
    /// <returns>True si es válido para conversión</returns>
    public static bool IsValidForDomainConversion(StudentDataModel dataModel)
    {
        if (dataModel == null)
            return false;

        try
        {
            // Intentar crear los Value Objects principales sin lanzar excepciones
            if (string.IsNullOrWhiteSpace(dataModel.Nombre) || 
                string.IsNullOrWhiteSpace(dataModel.Apellido))
                return false;

            if (string.IsNullOrWhiteSpace(dataModel.Dni))
                return false;

            if (string.IsNullOrWhiteSpace(dataModel.Email))
                return false;

            // Validaciones adicionales se pueden agregar aquí
            return true;
        }
        catch
        {
            return false;
        }
    }
}
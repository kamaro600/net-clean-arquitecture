using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Mappers;

/// <summary>
/// Mapper para convertir entre StudentCareer (Domain) y StudentCareerDataModel (Infrastructure)
/// </summary>
public class StudentCareerMapper
{
    /// <summary>
    /// Convierte de DataModel a Domain Entity
    /// </summary>
    public StudentCareer DataModelToDomain(StudentCareerDataModel dataModel)
    {
        var student = dataModel.Student != null ? 
            StudentMapper.ToDomain(dataModel.Student) : null;
        
        var career = dataModel.Career != null ? 
            CareerMapper.ToDomain(dataModel.Career) : null;

        return StudentCareer.Create(
            dataModel.StudentId,
            dataModel.CareerId,
            dataModel.EnrollmentDate,
            dataModel.IsActive,
            student,
            career
        );
    }

    /// <summary>
    /// Convierte de Domain Entity a DataModel
    /// </summary>
    public StudentCareerDataModel DomainToDataModel(StudentCareer domainEntity)
    {
        return new StudentCareerDataModel
        {
            StudentId = domainEntity.StudentId,
            CareerId = domainEntity.CareerId,
            EnrollmentDate = domainEntity.EnrollmentDate,
            IsActive = domainEntity.IsActive
            // No incluimos Student y Career para evitar referencias circulares
            // estas se cargarán por Entity Framework cuando sea necesario
        };
    }
}
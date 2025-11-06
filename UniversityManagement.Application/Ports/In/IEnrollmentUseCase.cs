using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;

namespace UniversityManagement.Application.Ports.In
{
    /// <summary>
    /// Puerto de entrada para casos de uso de matrícula
    /// </summary>
    public interface IEnrollmentUseCase
    {
        Task<EnrollmentResponse> EnrollStudentInCareerAsync(EnrollStudentCommand command);
        Task<EnrollmentResponse> UnenrollStudentFromCareerAsync(UnenrollStudentCommand command);
        Task<IEnumerable<EnrollmentResponse>> GetStudentEnrollmentsAsync(int studentId);
        Task<IEnumerable<EnrollmentResponse>> GetCareerEnrollmentsAsync(int careerId);
    }
}

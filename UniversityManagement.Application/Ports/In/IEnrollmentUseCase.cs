using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Application.DTOs.Responses;

namespace UniversityManagement.Application.Ports.In
{
    public interface IEnrollmentUseCase
    {
        Task<DeletionResponse> EnrollStudentInCareerAsync(int studentId, int careerId);
        Task<DeletionResponse> UnenrollStudentFromCareerAsync(int studentId, int careerId);
    }
}

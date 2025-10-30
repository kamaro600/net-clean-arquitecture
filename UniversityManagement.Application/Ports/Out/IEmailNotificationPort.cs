using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.Ports.Out
{
    public interface IEmailNotificationPort
    {
        Task SendEnrollmentConfirmation(String email, String ownerName,
                                   String course, String enrollmentDate);
        Task SendEnrollmentCancellation(String email, String ownerName,
                                         String course, String enrollmentDate);
        Task SendWelcomeAsync(string email, string fullName);

        Task SendStudentUpdateNotificationAsync(string email, string nombre, List<string> eventos);
    }
}

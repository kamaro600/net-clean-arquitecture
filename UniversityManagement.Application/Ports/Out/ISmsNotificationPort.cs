using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversityManagement.Application.Ports.Out
{
    public interface ISmsNotificationPort
    {
        Task SendMessageAsync(string phone, string message);
    }
}

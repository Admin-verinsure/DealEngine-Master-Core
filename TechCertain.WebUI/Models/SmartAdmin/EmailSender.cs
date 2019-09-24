using System.Threading.Tasks;

namespace TechCertain.WebUI.Models.SmartAdmin
{
    public class EmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
    }
}

using System.Threading.Tasks;

namespace TechCertain.WebUI.Models
{
    public class EmailSender 
    {
        public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
    }
}

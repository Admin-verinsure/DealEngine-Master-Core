using System.Threading.Tasks;

namespace DealEngine.WebUI.Models
{
    public class EmailSender 
    {
        public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
    }
}

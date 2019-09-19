using System.Threading.Tasks;

namespace techcertain2019core.Models.ViewModels.SmartAdmin
{
    public class EmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message) => Task.CompletedTask;
    }
}

using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IUWMService
    {
        bool UWM(User createdBy, ClientInformationSheet sheet, string reference);

    }
}
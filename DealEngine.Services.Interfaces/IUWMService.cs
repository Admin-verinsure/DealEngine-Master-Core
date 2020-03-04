using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IUWMService
    {
        bool UWM(User createdBy, ClientInformationSheet sheet, string reference);

    }
}
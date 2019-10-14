using TechCertain.Domain.Entities;

namespace TechCertain.Infrastructure.Payment.EGlobalAPI
{
    public interface ISerializer
    {
        string SerializePolicy(ClientProgramme clientProgramme, User currentUser);
    }
}
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
    public interface IActivityService
    {
        Task<Activity> GetActivity(string Name);
    }
}

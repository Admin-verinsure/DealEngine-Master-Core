using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IUnlockProcessService
    {

        Task CreateUnlockReason(User createdBy, UnlockReason changeReason);


    }
}


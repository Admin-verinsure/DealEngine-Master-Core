using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IChangeProcessService
    {

        Task CreateChangeReason(User createdBy,ChangeReason changeReason);


    }
}


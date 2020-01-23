using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IChangeProcessService
    {

        Task CreateChangeReason(User createdBy,ChangeReason changeReason);


    }
}


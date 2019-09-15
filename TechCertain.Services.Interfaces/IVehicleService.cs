using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IVehicleService
	{
		Vehicle GetValidatedVehicle (string plate);
	}
}


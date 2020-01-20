using System;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IVehicleService
	{
		Vehicle GetValidatedVehicle (string plate);
        Task<Vehicle> GetVehicleById(Guid vehicleId);
    }
}


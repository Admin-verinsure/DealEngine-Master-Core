using System;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IVehicleService
	{
		Vehicle GetValidatedVehicle (string plate);
        Task<Vehicle> GetVehicleById(Guid vehicleId);
    }
}


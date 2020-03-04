
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface ICountryService
	{
		Task<List<Country>> GetAllCountries ();

		bool AddCountry (Country country);
	}
}



using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface ICountryService
	{
		Task<List<Country>> GetAllCountries ();

		bool AddCountry (Country country);
	}
}


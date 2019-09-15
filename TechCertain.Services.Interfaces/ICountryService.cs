
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface ICountryService
	{
		List<Country> GetAllCountries ();

		bool AddCountry (Country country);
	}
}


using System;
using TechCertain.Services.Interfaces;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using System.Linq;

namespace TechCertain.Services.Impl
{
	public class CountryService : ICountryService
	{
		//private readonly IReadWriteRepository<Country> _countryRepository;
		//private readonly IUnitOfWork _unitOfWork;

		public CountryService ()//(IReadWriteRepository<Country> countryRepository, IUnitOfWork unitOfWork)
		{
			//_countryRepository = countryRepository;
			//_unitOfWork = unitOfWork;
		}

		#region ICountryService implementation

		public List<Country> GetAllCountries ()
		{
            //List<Country> list = _countryRepository.All ().ToList ();
            //list.OrderBy (o => o.Name);

            //return list;

            return new List<Country>();
		}

		public bool AddCountry (Country country)
		{
            //if (_countryRepository.Add (country)) {
            //    _unitOfWork.Commit ();
            //} else {
            //    _unitOfWork.Rollback ();
            //    return false;
            //}

			return false;
		}

		#endregion
	}
}
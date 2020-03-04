using System;
using DealEngine.Domain.Entities;
using System.Linq;

namespace DealEngine.Services.Interfaces
{
	public interface ICompanyService
	{
		IQueryable<Organisation> GetCompanies ();

		Organisation GetCompany (Guid guidId);

		void CreateNewCompany (Organisation company);

		void UpdateExistingCompany (Organisation company);

		void DeleteCompany (Organisation company);
	}
}


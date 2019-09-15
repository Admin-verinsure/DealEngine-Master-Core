using System;
using TechCertain.Domain.Entities;
using System.Linq;

namespace TechCertain.Services.Interfaces
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


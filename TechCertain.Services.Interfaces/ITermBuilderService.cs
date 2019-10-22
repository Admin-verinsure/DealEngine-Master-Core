using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface ITermBuilderService
	{
		Task<PolicyTermSection> Create (User createdBy, string name, string description, string version, int revision, string content, Guid creator, Guid territory, Guid jurisdiction);

        Task<PolicyTermSection> GetTerm (Guid termId);

        Task<List<PolicyTermSection>> GetTerms ();

        Task<List<PolicyTermSection>> GetTerms (string orderField, string direction);

        Task<bool> Deprecate (User deletedBy, Guid termId);
    }
}


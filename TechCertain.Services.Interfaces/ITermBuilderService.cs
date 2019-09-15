using System;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface ITermBuilderService
	{
		PolicyTermSection Create (User createdBy, string name, string description, string version, int revision, string content, Guid creator, Guid territory, Guid jurisdiction);

		PolicyTermSection GetTerm (Guid termId);

		PolicyTermSection[] GetTerms ();

		PolicyTermSection[] GetTerms (string orderField, string direction);

		bool Deprecate (User deletedBy, Guid termId);
	}
}


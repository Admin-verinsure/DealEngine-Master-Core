using System;
using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Linq;
using System.Linq.Dynamic;


namespace TechCertain.Services.Impl
{
	public class TermBuilderService : ITermBuilderService
	{
		IMapperSession<PolicyTermSection> _policyTermRepository;

		#region ITermBuilderService implementation

		public TermBuilderService(IMapperSession<PolicyTermSection> policyTermRepository)
		{
			_policyTermRepository = policyTermRepository;
		}

		public PolicyTermSection Create (User createdBy, string name, string description, string version, int revision, string content, Guid creator, Guid territory, Guid jurisdiction)
		{
			PolicyTermSection section = new PolicyTermSection (createdBy);
			section.Clause = Guid.Empty;
			section.Content = content;
			section.Creator = creator;
			section.Description = description;
			section.Jurisdiction = jurisdiction;
			section.Name = name;
			section.Owner = section.Creator;	// for now
			section.Revision = revision;
			section.Territory = territory;
			section.Version = version;

            _policyTermRepository.AddAsync(section);

            return section;
		}

		public PolicyTermSection GetTerm (Guid termId)
		{
			return _policyTermRepository.GetByIdAsync(termId).Result;
		}

		public PolicyTermSection[] GetTerms()
		{
			return _policyTermRepository.FindAll ().ToArray ();
		}

		public PolicyTermSection[] GetTerms(string orderField, string direction)
		{
            return _policyTermRepository.FindAll().OrderBy(orderField + " " + direction).ToArray();
		}

		public bool Deprecate (User deletedBy, Guid termId)
		{
			PolicyTermSection term = GetTerm (termId);
            _policyTermRepository.AddAsync(term);

            return GetTerm (termId).DateDeleted != null;
		}

		#endregion
		
	}
}


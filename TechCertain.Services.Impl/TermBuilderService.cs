using System;
using TechCertain.Services.Interfaces;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using System.Linq;
using System.Linq.Dynamic;


namespace TechCertain.Services.Impl
{
	public class TermBuilderService : ITermBuilderService
	{
		IUnitOfWorkFactory _unitOfWork;
		IRepository<PolicyTermSection> _policyTermRepository;

		#region ITermBuilderService implementation

		public TermBuilderService(IUnitOfWorkFactory unitOfWork, IRepository<PolicyTermSection> policyTermRepository)
		{
			_unitOfWork = unitOfWork;
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

			using (var uow = _unitOfWork.BeginUnitOfWork())
			{
				_policyTermRepository.Add(section);
				uow.Commit();
			}
			return section;
		}

		public PolicyTermSection GetTerm (Guid termId)
		{
			return _policyTermRepository.GetById (termId);
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
			using (var uow = _unitOfWork.BeginUnitOfWork())
			{
				term.Delete (deletedBy);
				_policyTermRepository.Add (term);
				uow.Commit();
			}
			return GetTerm (termId).DateDeleted != null;
		}

		#endregion
		
	}
}


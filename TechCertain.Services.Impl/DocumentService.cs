using System;
using System.Linq;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;

namespace TechCertain.Services.Impl
{
	public class DocumentService : IDocumentService
	{
		IMapperSession<PolicyDocumentTemplate> _policyDocumentRepository;
		[Obsolete]
		IMapperSession<Old_PolicyDocumentTemplate> _old_policyDocumentRepository;

		public DocumentService (IMapperSession<PolicyDocumentTemplate> policyDocumentRepository, 
			IMapperSession<Old_PolicyDocumentTemplate> old_policyDocumentRepository)
		{
			_policyDocumentRepository = policyDocumentRepository;
			_old_policyDocumentRepository = old_policyDocumentRepository;
		}

		#region Old IPolicyDocumentService implementation

		public Old_PolicyDocumentTemplate SaveDocumentTemplate (Old_PolicyDocumentTemplate policyDocument)
		{
            _old_policyDocumentRepository.AddAsync(policyDocument);
			return policyDocument;
		}

		public Old_PolicyDocumentTemplate GetDocumentTemplate (Guid id)
		{
			return _old_policyDocumentRepository.GetByIdAsync(id).Result;
		}

		public IList<Old_PolicyDocumentTemplate> GetDocumentTemplates()
		{
			List<Old_PolicyDocumentTemplate> documents = _old_policyDocumentRepository.FindAll ().ToList ();

			return documents;
		}

		#endregion

		#region IPolicyDocumentService implementation

		public PolicyDocumentTemplate CreateDocumentTemplate(User createdBy, string documentTitle, IList<PolicyTermSection> sections)
		{
			PolicyDocumentTemplate template = new PolicyDocumentTemplate (createdBy, documentTitle, "", "", Guid.Empty, Guid.Empty, false);
			foreach (PolicyTermSection section in sections)
				template.AddSection (section);


			return template;
		}

		public IEnumerable<PolicyDocumentTemplate> GetAllTemplates()
		{
			return _policyDocumentRepository.FindAll ().AsEnumerable ();
		}

		public string RenderDocument(string documentTitle, List<KeyValuePair<string, string>> mergeFields)
		{
			Old_PolicyDocumentTemplate template = GetDocumentTemplates ().Where (t => t.Title == documentTitle).OrderByDescending (t => t.Revision).FirstOrDefault ();

			if (template == null)
				return "";

			string content = template.Text;
			foreach (KeyValuePair<string, string> field in mergeFields)
				content = content.Replace (field.Key, field.Value);

			return content;
		}

        #endregion
    }
}


using System;
using System.Linq;
using System.Collections.Generic;
using TechCertain.Domain.Entities;
using TechCertain.Services.Interfaces;
using TechCertain.Infrastructure.FluentNHibernate;
using System.Threading.Tasks;
using NHibernate.Linq;

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

		public async Task<Old_PolicyDocumentTemplate> SaveDocumentTemplate (Old_PolicyDocumentTemplate policyDocument)
		{
            await _old_policyDocumentRepository.AddAsync(policyDocument);
			return policyDocument;
		}

		public async Task<Old_PolicyDocumentTemplate> GetDocumentTemplate (Guid id)
		{
			return await _old_policyDocumentRepository.GetByIdAsync(id);
		}

		public async Task<List<Old_PolicyDocumentTemplate>> GetDocumentTemplates()
		{
			List<Old_PolicyDocumentTemplate> documents = await _old_policyDocumentRepository.FindAll().ToListAsync();

			return documents;
		}

		#endregion

		#region IPolicyDocumentService implementation

		public async Task<PolicyDocumentTemplate> CreateDocumentTemplate(User createdBy, string documentTitle, IList<PolicyTermSection> sections)
		{
			PolicyDocumentTemplate template = new PolicyDocumentTemplate (createdBy, documentTitle, "", "", Guid.Empty, Guid.Empty, false);
			foreach (PolicyTermSection section in sections)
				template.AddSection (section);


			return template;
		}

		public async Task<List<PolicyDocumentTemplate>> GetAllTemplates()
		{
			return await _policyDocumentRepository.FindAll().ToListAsync();
		}

		public async Task<string> RenderDocument(string documentTitle, List<KeyValuePair<string, string>> mergeFields)
		{
            var templateList = await GetDocumentTemplates();
            Old_PolicyDocumentTemplate template = templateList.Where(t => t.Title == documentTitle).OrderByDescending(t => t.Revision).FirstOrDefault();

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


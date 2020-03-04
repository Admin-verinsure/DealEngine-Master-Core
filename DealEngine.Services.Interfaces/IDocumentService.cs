using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
	public interface IDocumentService
	{
        #region old
        Task<Old_PolicyDocumentTemplate> SaveDocumentTemplate(Old_PolicyDocumentTemplate policyDocument);

        Task<Old_PolicyDocumentTemplate> GetDocumentTemplate (Guid id);

		Task<List<Old_PolicyDocumentTemplate>> GetDocumentTemplates();
		#endregion

		Task<PolicyDocumentTemplate> CreateDocumentTemplate(User createdBy, string documentTitle, IList<PolicyTermSection> sections);

		Task<List<PolicyDocumentTemplate>> GetAllTemplates();

		Task<string> RenderDocument (string documentTitle, List<KeyValuePair<string, string>> mergeFields);
    }
}


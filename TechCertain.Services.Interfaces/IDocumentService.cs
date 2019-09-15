using System;
using System.Collections.Generic;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
{
	public interface IDocumentService
	{
		#region old
		Old_PolicyDocumentTemplate SaveDocumentTemplate(Old_PolicyDocumentTemplate policyDocument);

		Old_PolicyDocumentTemplate GetDocumentTemplate (Guid id);

		IList<Old_PolicyDocumentTemplate> GetDocumentTemplates();
		#endregion

		PolicyDocumentTemplate CreateDocumentTemplate(User createdBy, string documentTitle, IList<PolicyTermSection> sections);

		IEnumerable<PolicyDocumentTemplate> GetAllTemplates();

		string RenderDocument (string documentTitle, List<KeyValuePair<string, string>> mergeFields);
    }
}


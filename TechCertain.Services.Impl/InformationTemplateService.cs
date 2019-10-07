using System;
using System.Collections.Generic;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class InformationTemplateService : IInformationTemplateService
    {
        //private InformationTemplateFactory _informationTemplateFactory;
        private IMapperSession<InformationTemplate> _informationTemplateRepository;

        public InformationTemplateService(IMapperSession<InformationTemplate> informationTemplateRepository)
        {
            _informationTemplateRepository = informationTemplateRepository;
        }

        public InformationTemplate CreateInformationTemplate(User createdBy, string name, IList<InformationSection> sections)
        {
            InformationTemplate template = new InformationTemplate(createdBy, name, sections); //_informationTemplateFactory.CreateNewTemplate(createdBy, name, sections);

            _informationTemplateRepository.Add(template);

            return template;
        }

        public IQueryable<InformationTemplate> GetAllTemplates()
        {
            return _informationTemplateRepository.FindAll();
        }

		public InformationTemplate GetTemplate (Guid templateId)
		{
			return _informationTemplateRepository.GetById (templateId);
		}

		public InformationTemplate AddProductTo (Guid templateId, Product product)
		{
			InformationTemplate template = GetTemplate (templateId);
			if (template == null)
				throw new NullReferenceException ("Unable to find Information Sheet Template for " + templateId);

			return AddProductTo (template, product);
		}

		public InformationTemplate AddProductTo (InformationTemplate template, Product product)
		{
			if (template.Product != null)
				throw new Exception ("There is already a product assigned to this Information Sheet Template");
			template.Product = product;

			product.UniqueQuestions = template.Sections.Where (s => string.IsNullOrWhiteSpace (s.CustomView)).ToList();
			product.SharedViews = template.Sections.Where (s => !string.IsNullOrWhiteSpace (s.CustomView)).ToList ();

			//product.InformationTemplate = template;

			_informationTemplateRepository.Add (template);

			return template;
		}
	}
}

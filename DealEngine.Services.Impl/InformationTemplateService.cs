using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;
using DealEngine.Infrastructure.FluentNHibernate;
using DealEngine.Services.Interfaces;

namespace DealEngine.Services.Impl
{
    public class InformationTemplateService : IInformationTemplateService
    {
        //private InformationTemplateFactory _informationTemplateFactory;
        private IMapperSession<InformationTemplate> _informationTemplateRepository;

        public InformationTemplateService(IMapperSession<InformationTemplate> informationTemplateRepository)
        {
            _informationTemplateRepository = informationTemplateRepository;
        }

        public async Task<InformationTemplate> CreateInformationTemplate(User createdBy, string name, IList<InformationSection> sections)
        {
            InformationTemplate template = new InformationTemplate(createdBy, name, sections); //_informationTemplateFactory.CreateNewTemplate(createdBy, name, sections);

            await _informationTemplateRepository.AddAsync(template);

            return template;
        }
        public async Task<InformationTemplate> GetTemplatebyProduct(Guid productId)
        {
            return await _informationTemplateRepository.FindAll().SingleOrDefaultAsync(I => I.Product.Id== productId );
        }

        public async Task<List<InformationTemplate>> GetAllTemplatesbyproduct(Guid productId)
        {
            return await _informationTemplateRepository.FindAll().Where(I => I.Product.Id == productId).ToListAsync();
        }

        public async Task<List<InformationTemplate>> GetAllTemplates()
        {
            return await _informationTemplateRepository.FindAll().ToListAsync();
        }

		public async Task<InformationTemplate> GetTemplate (Guid templateId)
		{
			return await _informationTemplateRepository.GetByIdAsync(templateId);
		}

		public async Task<InformationTemplate> AddProductTo (Guid templateId, Product product)
		{
			InformationTemplate template = await GetTemplate(templateId);
			if (template == null)
				throw new NullReferenceException ("Unable to find Information Sheet Template for " + templateId);

			return await AddProductTo(template, product);
		}

		public async Task<InformationTemplate> AddProductTo (InformationTemplate template, Product product)
		{
			if (template.Product != null)
				throw new Exception ("There is already a product assigned to this Information Sheet Template");
			template.Product = product;

			product.UniqueQuestions = template.Sections.Where (s => string.IsNullOrWhiteSpace (s.CustomView)).ToList();
			product.SharedViews = template.Sections.Where (s => !string.IsNullOrWhiteSpace (s.CustomView)).ToList ();

			//product.InformationTemplate = template;

			await _informationTemplateRepository.AddAsync(template);

			return template;
		}
    }
}

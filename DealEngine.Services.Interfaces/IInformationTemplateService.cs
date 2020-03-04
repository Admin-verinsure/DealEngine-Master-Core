﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DealEngine.Domain.Entities;

namespace DealEngine.Services.Interfaces
{
    public interface IInformationTemplateService
    {
        Task<InformationTemplate> CreateInformationTemplate(User createdBy, string name, IList<InformationSection> sections);
        Task<List<InformationTemplate>> GetAllTemplates();        
        Task<List<InformationTemplate>> GetAllTemplatesbyproduct(Guid productId);
        Task<InformationTemplate> GetTemplate(Guid templateId);
        Task<InformationTemplate> GetTemplatebyProduct(Guid productId);
        Task<InformationTemplate> AddProductTo(Guid templateId, Product product);
        Task<InformationTemplate> AddProductTo(InformationTemplate template, Product product);        
    }
}

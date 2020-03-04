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
    public class EndorsementService : IEndorsementService
    {
        IMapperSession<Product> _productRepository;
        IMapperSession<Endorsement> _endorsementRepository;

        public EndorsementService(IMapperSession<Endorsement> endorsementRepository, IMapperSession<Product> productRepository)
        {
            _productRepository = productRepository;
            _endorsementRepository = endorsementRepository;
        }

        public async Task AddEndorsementAsync(User createdBy, string name, string type, Product product, string value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentNullException(nameof(type));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            Endorsement endorsement = new Endorsement(createdBy, name, type, product, value);
            product.Endorsements.Add(endorsement);
            await _endorsementRepository.AddAsync(endorsement);
            await _productRepository.UpdateAsync(product);
        }


        public async Task<List<Endorsement>> GetAllEndorsementFor(Product product)
        {
            return await _endorsementRepository.FindAll().Where(cagt => cagt.Product == product).ToListAsync();
        }
    }
}


using System;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
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

        public void AddEndorsementAsync(User createdBy, string name, string type, Product product, string value)
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
            _endorsementRepository.AddAsync(endorsement);
            _productRepository.UpdateAsync(product);
        }


        public IQueryable<Endorsement> GetAllEndorsementFor(Product product)
        {
            return _endorsementRepository.FindAll().Where(cagt => cagt.Product == product);
        }
    }
}


using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ProductService : IProductService
    {        
        public IMapperSession<Product> _productRepository;

        public ProductService(IMapperSession<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task CreateProduct(Product product)
        {
            await _productRepository.AddAsync(product);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _productRepository.FindAll().ToListAsync();
        }

        public async Task<Product> GetProductById(Guid Id)
        {
            return await _productRepository.GetByIdAsync(Id);
        }
    }
}

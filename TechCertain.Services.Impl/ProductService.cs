using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;
using TechCertain.Infrastructure.FluentNHibernate;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
{
    public class ProductService : IProductService
    {        
        public IMapperSession<Product> _ProductRepository;

        public ProductService(IMapperSession<Product> ProductRepository)
        {
            _ProductRepository = ProductRepository;
        }

        public  Product GetAllProducts()
        {
            return _ProductRepository.FindAll().FirstOrDefault();
        }

    }
}

using System;
using System.Linq;
using TechCertain.Domain.Entities;
using TechCertain.Domain.Interfaces;
using TechCertain.Services.Interfaces;

namespace TechCertain.Services.Impl
    {
        public class EndorsementService : IEndorsementService
    {
            IUnitOfWork _unitOfWork;
            IMapperSession<Endorsement> _endorsementRepository;

            public EndorsementService(IUnitOfWork unitOfWork, IMapperSession<Endorsement> endorsementRepository)
            {
                _unitOfWork = unitOfWork;
                _endorsementRepository = endorsementRepository;
            }

            public bool AddEndorsement(User createdBy, string name, string type, Product product, string value)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));
                if (string.IsNullOrWhiteSpace(type))
                    throw new ArgumentNullException(nameof(type));
                if (product == null)
                    throw new ArgumentNullException(nameof(product));
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(value));

                using (IUnitOfWork work = _unitOfWork.BeginUnitOfWork())
                {
                Endorsement endorsement = new Endorsement(createdBy, name, type, product, value);
                    product.Endorsements.Add(endorsement);
                    work.Commit();
                }

                return true;
            }


            public IQueryable<Endorsement> GetAllEndorsementFor(Product product)
            {
                var endorsement = _endorsementRepository.FindAll().Where(cagt => cagt.Product == product);
                return endorsement;
            }
        }
    }


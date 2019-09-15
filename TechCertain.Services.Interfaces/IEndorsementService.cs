using System.Linq;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
    {
        public interface IEndorsementService
    {
            bool AddEndorsement(User createdBy, string name, string type, Product product, string value);

            IQueryable<Endorsement> GetAllEndorsementFor(Product product);
        }
    }

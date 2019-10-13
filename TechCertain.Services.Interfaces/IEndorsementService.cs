using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
    {
        public interface IEndorsementService
    {
            void AddEndorsementAsync(User createdBy, string name, string type, Product product, string value);

            IQueryable<Endorsement> GetAllEndorsementFor(Product product);
        }
    }

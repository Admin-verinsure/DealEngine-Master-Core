using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechCertain.Domain.Entities;

namespace TechCertain.Services.Interfaces
    {
        public interface IEndorsementService
    {
            Task AddEndorsementAsync(User createdBy, string name, string type, Product product, string value);

            Task<List<Endorsement>> GetAllEndorsementFor(Product product);
        }
    }

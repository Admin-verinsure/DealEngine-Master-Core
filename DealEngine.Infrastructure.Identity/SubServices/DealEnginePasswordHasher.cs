
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity.Subservices
{
    public class DealEnginePasswordHasher : IPasswordHasher<User>
    {
        public string HashPassword(User user, string password)
        {
            throw new System.NotImplementedException();
        }

        public PasswordVerificationResult VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        {
            throw new System.NotImplementedException();
        }
    }
}


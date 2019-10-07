using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechCertain.Domain.Entities;

namespace DealEngine.Infrastructure.Identity.Data
{
    public class DealEngineDBContext : IdentityDbContext<DealEngineUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DealEngineDBContext(DbContextOptions<DealEngineDBContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Ignore(typeof(User));
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

        }
    }
}

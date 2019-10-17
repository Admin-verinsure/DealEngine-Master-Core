using DealEngine.Infrastructure.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


[assembly: HostingStartup(typeof(TechCertain.WebUI.Areas.Identity.IdentityHostingStartup))]
namespace TechCertain.WebUI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<DealEngineDBContext>(options =>
                    options.UseNpgsql(
                        context.Configuration.GetConnectionString("TechCertainConnection")));

                services.AddIdentity<DealEngineUser, IdentityRole>(options =>
                    {
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireDigit = false;
                        options.Password.RequiredLength = 6;
                        options.Password.RequireLowercase = false;
                    })

                    .AddEntityFrameworkStores<DealEngineDBContext>();
            });
        }
    }

}
using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechCertain.WebUI.Areas.Identity.Data;

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

                services.AddDefaultIdentity<DealEngineUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddClaimsPrincipalFactory<DealEngineClaimsPrincipalFactory>()
                    .AddEntityFrameworkStores<DealEngineDBContext>();
            });
        }
    }

}
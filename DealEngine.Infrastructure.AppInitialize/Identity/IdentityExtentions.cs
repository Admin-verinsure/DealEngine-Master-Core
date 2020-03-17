using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NHibernate.AspNetCore.Identity;
using IdentityRole = NHibernate.AspNetCore.Identity.IdentityRole;
using IdentityUser = NHibernate.AspNetCore.Identity.IdentityUser;

namespace DealEngine.Infrastructure.AppInitialize
{
    public static class IdentityExtentions
    {
        public static IServiceCollection AddIdentityExtentions(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+'";
            })
                .AddSignInManager<SignInManager<IdentityUser>>()
                .AddUserManager<UserManager<IdentityUser>>()  
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddHibernateStores();

            return services;
        }
    }
}

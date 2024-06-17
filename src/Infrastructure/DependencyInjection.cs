using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using MyPosTask.Infrastructure.Persistence;
using MyPosTask.Infrastructure.Data.Interceptors;
using MyPosTask.Domain.Constants;
using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var myPosConnectionString = configuration.GetConnectionString("DefaultConnection");
            Guard.Against.NullOrWhiteSpace(myPosConnectionString, nameof(myPosConnectionString), "Connection string 'MyPosDatabaseConnection' not found.");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

            services.AddAuthorization(options =>
                options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator)));

            return services;
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VotRomania.Stores;

namespace VotRomania.Extensions
{
    public static class PostgreSqlExtension
    {
        public static void AddPostgreSqlDbContext(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContextPool<VotRomaniaContext>(options =>
                options.UseNpgsql(config.GetConnectionString("VotRoDBConnection"), sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null
                    );
                }));
        }
    }
}

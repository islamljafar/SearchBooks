//using ElmTest.Application.Interfaces;
//using ElmTest.Application.Services;
using ElmTest.Domain.Interfaces;
using ElmTest.Infrastructure;
using ElmTest.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using System.Data.SqlClient;

namespace ElmTest.Infrastructure.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IDbConnection>(provider => new SqlConnection(connectionString));
            services.AddScoped<IBookRepository, BookRepository>();
            return services;
        }
    }
}

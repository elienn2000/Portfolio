using Dapper.SimpleRepository;
using Microsoft.Data.Sqlite;
using System.Data;
using PortfolioApi.Repositories;
using PortfolioApi.Models; // Assicurati che questo sia il namespace corretto

namespace PortfolioApi.Helper
{
    public static class ServiceExtension
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {

            /////////////////////// Repositories ///////////////////////
            services.AddScoped<UserRepository>();

            /////////////////////////// Data ///////////////////////////
            services.AddScoped<User>();

        }
    }
}

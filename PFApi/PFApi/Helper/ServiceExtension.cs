using Dapper.SimpleRepository;
using Microsoft.Data.Sqlite;
using PortfolioApi.Models; // Assicurati che questo sia il namespace corretto
using PortfolioApi.Repositories;
using PortfolioApi.Services;
using System.Data;

namespace PortfolioApi.Helper
{
    public static class ServiceExtension
    {
        public static void ConfigureRepositoryWrapper(this IServiceCollection services)
        {

            ////////////////////////    Services  ////////////////////////
            services.AddScoped<EmailService>();
            services.AddScoped<EmailVerificationService>();


            /////////////////////// Repositories ///////////////////////
            services.AddScoped<UserRepository>();

            /////////////////////////// Data ///////////////////////////
            services.AddScoped<User>();

        }
    }
}

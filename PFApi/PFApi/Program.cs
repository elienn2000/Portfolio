using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.Sqlite;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PortfolioApi.Helper;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Settings.Configuration; // AGGIUNTO: necessario per ReadFrom.Configuration
using System.Data;
using System.Text;

namespace PortfolioApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config) // ora funziona grazie al using aggiunto sopra
                .Enrich.FromLogContext()
                .CreateLogger();

            Log.Logger = logger;
            logger.Information("Starting up");

            // Carica le impostazioni JWTUser da appsettings.json
            var jwtUserSettings = config.GetSection("JWTUser").Get<JwtUserSettings>();
            if (jwtUserSettings == null)
            {
                throw new InvalidOperationException("Impossibile caricare la configurazione JWTUser da appsettings.json.");
            }


            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            var connString = builder.Configuration.GetConnectionString("DefaultConnection");

            if (connString == null)
            {
                throw new InvalidOperationException("Impossibile caricare la configurazione JWTUser da appsettings.json.");
            }

            // Verifica e crea il database SQLite se non esiste
            DbChecker.EnsureDatabaseExists(connString);

            // Connessione sql lite al file del database
            builder.Services.AddScoped<IDbConnection>(sp =>
                new SqliteConnection(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.ConfigureRepositoryWrapper();

            // Autenticazione JWTUser
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JWTUser";
                options.DefaultChallengeScheme = "JWTUser";
            })
            .AddJwtBearer("JWTUser", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtUserSettings.Issuer,
                    ValidAudience = jwtUserSettings.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSettings.Secret)),
                };
            });

            // Autorizzazione
            builder.Services.AddAuthorization();

            // Aggiunta di OpenAPI/Swagger per documentazione API
            builder.Services.AddOpenApi();

            // Configurazione di Swagger con supporto per l'autenticazione JWT
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.AddSecurityDefinition("JWTUser", new OpenApiSecurityScheme
                {
                    Description = "Token JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "JWTUser"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Configurazione CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Open", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Aggiunta dei controller
            builder.Services.AddControllers();

            // Aggiunta di SignalR
            builder.Services.AddSignalR();


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("Open");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // app.MapHub<HubSignalR>("/notificheHub");

            app.Run();
        }
    }
}

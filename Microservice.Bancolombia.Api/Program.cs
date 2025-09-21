using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microservice.Bancolombia.Api.Entities;
using Microservice.Bancolombia.Api.Properties;
using Microservice.Bancolombia.Api.Services.Interfaces;
using Microservice.Bancolombia.Api.Services;
using Microservice.Bancolombia.Api.Clients.Interfaces;
using Microservice.Bancolombia.Api.Clients;
using Microservice.Bancolombia.Api.Exceptions;

namespace Microservice.Bancolombia.Api
{
    /// <summary>
    /// The main program class for Bancolombia API.
    /// </summary>
    internal class Program
    {
        protected Program() { }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            SetBuilderConfiguration(builder);
            SetAppConfiguration(builder);
        }

        /// <summary>
        /// Configures the builder services.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        public static void SetBuilderConfiguration(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            ConfigureSwagger(builder);

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            ConfigureConnectionDataBase(builder);
            ConfigureServices(builder);
            ConfigureClients(builder);
            ConfigureCorsPolicy(builder);
        }

        /// <summary>
        /// Configures the application pipeline.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        public static void SetAppConfiguration(WebApplicationBuilder builder)
        {
            WebApplication app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bancolombia API V1");
                    c.DocumentTitle = "Bancolombia Microservice API Documentation";
                });
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("CorsOrigins");
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.Run();
        }

        /// <summary>
        /// Configures Swagger documentation.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        private static void ConfigureSwagger(WebApplicationBuilder builder)
        {
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Bancolombia Microservice API",
                    Version = "v1",
                    Description = "API para gestión de cuentas y transacciones bancarias de Bancolombia"
                });

                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        /// <summary>
        /// Configures the database connection.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        public static void ConfigureConnectionDataBase(IHostApplicationBuilder builder)
        {
            string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new DatabaseConfigurationException();

            builder.Services.AddDbContext<MainContext>(options =>
                options.UseSqlServer(connectionString));
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<ITransactionHistoryService, TransactionHistoryService>();
        }

        /// <summary>
        /// Configures the clients for the application.
        /// </summary>
        /// <param name="builder">The web application builder.</param>
        private static void ConfigureClients(WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<IAccountClient>(provider =>
            {
                var context = provider.GetRequiredService<MainContext>();
                return new AccountClient(context);
            });

            builder.Services.AddTransient<ITransactionHistoryClient>(provider =>
            {
                var context = provider.GetRequiredService<MainContext>();
                return new TransactionHistoryClient(context);
            });
        }

        /// <summary>
        /// Configures the CORS policy for the application.
        /// </summary>
        /// <param name="builder">The host application builder.</param>
        private static void ConfigureCorsPolicy(IHostApplicationBuilder builder)
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsOrigins", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
        }
    }
}
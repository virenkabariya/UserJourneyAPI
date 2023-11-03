namespace UserJourney.API.Middleware
{
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using UserJourney.API.Code;
    using UserJourney.API.Contracts;
    using UserJourney.API.Services;
    using UserJourney.Repositories.Concrete;
    using UserJourney.Repositories.Contracts;
    using UserJourney.Repositories.CustomException;
    using UserJourney.Repositories.EF;

    public static class DependencyInjectionMiddleware
    {
        public static IServiceCollection RegisterAllDependencies(this IServiceCollection serviceCollection, IConfiguration config)
        {
            if (serviceCollection is null)
            {
                throw new ArgumentNullException(nameof(serviceCollection));
            }

            if (config is null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            serviceCollection.AddDbContext<UserJourneyContext>(options =>
                options.UseSqlServer(config.GetConnectionString("UserJourneyDB")));
            serviceCollection.Configure<EMailSettings>(options => config.GetSection("EMailSettings").Bind(options));
            serviceCollection.AddTransient<IEmailService, EmailService>();
            serviceCollection.AddScoped<IProjectUnitOfWork, ProjectUnitOfWork>();
            

            serviceCollection.AddProblemDetails(setup =>
            {
                setup.Map<CustomException>(exception => new CustomExceptionDetails
                {
                    Title = exception.Title,
                    Detail = exception.Detail,
                    Status = StatusCodes.Status500InternalServerError,
                    Type = exception.Type,
                    Instance = exception.Instance,
                    AdditionalInfo = exception.AdditionalInfo
                });
            });

            return serviceCollection;
        }
    }
}

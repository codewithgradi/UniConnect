using Microsoft.Extensions.DependencyInjection;
using UniConnect.Application.Services;
using UniConnect.Domain.Interfaces.Repositories;
using UniConnect.Infrastructure.Repositories;

namespace UniConnect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IBusinessProfileRepository, BusinessProfileRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IConnectionRepository, ConnectionRepository>();
        services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
        services.AddScoped<IOpportunityRepository, OpportunityRepository>();
        services.AddScoped<IInstitutionalEventRepository, InstitutionalEventRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IUserAnalyticsRepository, UserAnalyticsRepository>();
        services.AddScoped<ISkillRepository, SkillRepository>();


        return services;
    }
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IBusinessService, BusinessService>();
        services.AddScoped<ISkillService, SkillService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IConnectionService, ConnectionService>();
        services.AddScoped<IMessagingService, MessagingService>();
        services.AddScoped<IOpportunityService, OpportunityService>();
        services.AddScoped<IInstitutionalService, InstitutionalService>();
        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IUserAnalyticsService, UserAnalyticsService>();

        return services;
    }
}
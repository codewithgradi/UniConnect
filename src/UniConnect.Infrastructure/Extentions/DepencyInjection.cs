using System.ClientModel;
using Amazon.S3;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenAI;
using UniConnect.Application.Services;
using UniConnect.Domain.Interfaces.Repositories;
using UniConnect.Infrastructure.AwsS3;
using UniConnect.Infrastructure.Repositories;

namespace UniConnect.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGlobalException(this IServiceCollection services)
    {
        services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = context =>
            {
                if (context.Exception is KeyNotFoundException notFoundEx)
                {
                    context.ProblemDetails.Status = StatusCodes.Status404NotFound;
                    context.ProblemDetails.Title = "Resource Not Found";
                    context.ProblemDetails.Detail = notFoundEx.Message;
                }
                else if (context.Exception is InvalidOperationException businessRuleEx)
                {
                    context.ProblemDetails.Status = StatusCodes.Status400BadRequest;
                    context.ProblemDetails.Title = "Business Rule Exception";
                    context.ProblemDetails.Detail = businessRuleEx.Message;
                }
                else if (context.Exception is UnauthorizedAccessException unauthEx)
                {
                    context.ProblemDetails.Status = StatusCodes.Status401Unauthorized;
                    context.ProblemDetails.Title = "Unauthorized Access";
                    context.ProblemDetails.Detail = unauthEx.Message;
                }
            };
        });
        return services;
    }

    public static IServiceCollection AddEnvironmentVariables(this IServiceCollection services)
    {
        DotNetEnv.Env.TraversePath().Load();
        return services;
    }

    public static IServiceCollection LoadDb(this IServiceCollection services, IConfiguration configuration)
    {
        var env = configuration["env"]?.ToLower() ?? "dev";

        string? connectionString = env switch
        {
            "dev" => configuration.GetConnectionString("DevDB"),
            "prod" => configuration.GetConnectionString("ProdDB"),
            _ => throw new InvalidOperationException($"Unsupported environment: '{env}'")
        };

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException($"Missing connection string for environment: '{env}'");
        }

        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(connectionString));

        return services;
    }

    public static IServiceCollection AllowCors(this IServiceCollection services, IConfiguration configuration)
    {
        var frontendDevUrl = configuration["OtherSettings:FrontEndDevUrl"];
        var frontendProdUrl = configuration["OtherSettings:FrontEndProdUrl"];

        if (string.IsNullOrEmpty(frontendDevUrl) || string.IsNullOrEmpty(frontendProdUrl))
        {
            throw new InvalidOperationException("Missing front-end URLs in configuration.");
        }

        services.AddCors(opt =>
        {
            opt.AddPolicy("AllowNextJs", builder =>
            {
                builder.WithOrigins(frontendDevUrl, frontendProdUrl)
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            });
        });

        return services;
    }
    public static IServiceCollection AddOpenAI(this IServiceCollection services, IConfiguration configuration)
    {
        string apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing OpenAI:ApiKey in configuration.");

        string openRouterUrl = configuration["OpenAI:OpenRouterUrl"] ?? "https://openrouter.ai/api/v1";
        if (!openRouterUrl.EndsWith("/v1") && !openRouterUrl.EndsWith("/v1/"))
        {
            openRouterUrl = $"{openRouterUrl.TrimEnd('/')}/v1";
        }

        var openAiOptions = new OpenAIClientOptions
        {
            Endpoint = new Uri(openRouterUrl)
        };

        var openAiClient = new OpenAIClient(new ApiKeyCredential(apiKey), openAiOptions);
        string modelId = configuration["OpenAI:ModelId"] ?? "google/gemini-2.5-flash";

        IChatClient innerClient = openAiClient
            .GetChatClient(modelId)
            .AsIChatClient();

        IChatClient chatClient = new ChatClientBuilder(innerClient)
            .ConfigureOptions(options =>
            {
                options.MaxOutputTokens = 1000; // Enforces token limits globally
            })
            .UseFunctionInvocation()
            .Build();

        services.AddSingleton<IChatClient>(chatClient);

        return services;
    }
    public static IServiceCollection ConfigureMcp(this IServiceCollection services)
    {
        services.AddMcpServer()
            .WithHttpTransport(opt => opt.Stateless = true)
            .WithToolsFromAssembly();
        return services;
    }


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
        services.AddScoped<IR2StorageService, R2StorageService>();

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
        services.AddScoped<IUserAnalyticsService, UserAnalyticsService>();
        services.AddScoped<IAnalyticsService, AnalyticsService>();
        services.AddScoped<ICertificateVerificationService, CertificateVerificationService>();

        return services;
    }
}
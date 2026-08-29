using System.Text.Json.Serialization;
using Amazon.S3;
using Infrastructure.Persistence;
using Microsoft.OpenApi;
using UniConnect.Api.Mcp;
using UniConnect.Domain.Entities;
using UniConnect.Infrastructure;
using UniConnect.Infrastructure.Identity;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var accountId = config["R2:AccountId"];

    var s3Config = new AmazonS3Config
    {
        ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com"
    };

    return new AmazonS3Client(
        config["R2:AccessKeyId"],
        config["R2:SecretAccessKey"],
        s3Config
    );
});

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// 5. MCP Tool Registrations
builder.Services.AddScoped<UserProfileMcpTool>();

// 6. Infrastructure & Domain Service Registrations
builder.Services
    .AllowCors(builder.Configuration)
    .LoadDb(builder.Configuration)
    .AddGlobalException()
    .AddInfrastructureRepositories()
    .AddApplicationServices()
    .ConfigureMcp()
    .AddOpenAI(builder.Configuration);

// 7. OpenAPI Specification with Bearer JWT Setup
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info ??= new OpenApiInfo { Title = "UniConnect.Api", Version = "v1" };

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter your Bearer token below:",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = securityScheme;

        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

        if (document.Paths is not null)
        {
            foreach (var path in document.Paths)
            {
                if (path.Value?.Operations is null) continue;

                // Skip security requirements for public identity or auth endpoints
                if (path.Key.StartsWith("/api/identity", StringComparison.OrdinalIgnoreCase) ||
                    path.Key.StartsWith("/api/auth", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                foreach (var operation in path.Value.Operations.Values)
                {
                    operation.Security ??= new List<OpenApiSecurityRequirement>();
                    operation.Security.Add(new OpenApiSecurityRequirement
                    {
                        [schemeReference] = new List<string>()
                    });
                }
            }
        }

        return Task.CompletedTask;
    });
});

var app = builder.Build();

// 8. Middleware Pipeline Setup
app.UseExceptionHandler();
app.UseCors("AllowNextJs");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "UniConnect.Api v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 9. Endpoint Mappings
app.MapGroup("/api/identity")
   .MapIdentityApi<ApplicationUser>()
   .WithTags("Auth")
   .AllowAnonymous();

app.MapMcp("/mcp");
app.MapControllers();

app.Run();
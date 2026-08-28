using System.Text.Json.Serialization;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using UniConnect.Domain.Entities;
using UniConnect.Infrastructure;
using UniConnect.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

// 1. Add PostgreSQL DbContext
var connectionString = builder.Configuration.GetConnectionString("DevDB");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. Identity & Roles Configuration
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// 3. Infrastructure & Application Dependencies
builder.Services.AddInfrastructureRepositories().AddApplicationServices();

// 4. Configure JSON options for BOTH Controllers AND Minimal APIs/OpenAPI
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// 5. Configure Native OpenAPI with Bearer Authorization
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info ??= new OpenApiInfo();
        document.Info.Title = "UniConnect.Api";
        document.Info.Version = "v1";

        // Define Bearer Security Scheme
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

        // Apply security per operation instead of globally to leave auth endpoints public
        var schemeReference = new OpenApiSecuritySchemeReference("Bearer", document);

        if (document.Paths != null)
        {
            foreach (var path in document.Paths)
            {
                // Skip adding token requirement for identity or public auth routes
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

// 6. Configure HTTP Request Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Serves /openapi/v1.json
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "UniConnect.Api v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 7. Map Identity Auth Routes (Public)
app.MapGroup("/api/identity")
   .MapIdentityApi<ApplicationUser>()
   .WithTags("Auth")
   .AllowAnonymous();

// 8. Map Application Controllers
app.MapControllers();

app.Run();
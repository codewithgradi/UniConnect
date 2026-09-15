using System.Text.Json.Serialization;
using Amazon.S3;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi;
using UniConnect.Api.Hubs;
using UniConnect.Api.Mcp;
using UniConnect.Application.Interfaces;
using UniConnect.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UniConnect.Domain.Entities;
using UniConnect.Infrastructure;
using UniConnect.Infrastructure.Identity;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.Configure<BrevoSettings>(
    builder.Configuration.GetSection(BrevoSettings.SectionName));
// Register Redis Distributed Cache
builder.Services.AddDistributedMemoryCache(options =>
{
    // options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // options.InstanceName = "UniConnect_";
});
builder.Services.AddHttpClient();

// Register your EmailService implementation
builder.Services.AddHttpClient<IEmailService, EmailService>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Configuration.AddEnvironmentVariables();

// 1. Controller & JSON Configuration
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// 2. Identity & Database Context Setup
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

// 3. Cloudflare R2 S3 Client Setup
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

// 4. Real-time & Authentication Setup
// 4. Real-time & Authentication Setup
builder.Services.AddSignalR();

// Register JWT Bearer alongside Identity
builder.Services.AddAuthentication(options =>
{
    // Default scheme for [Authorize] controllers
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "UniConnectApi",

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "UniConnectApp",

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? builder.Configuration["JwtSettings:Secret"]!)),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };

    // SignalR WebSocket token extractor
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/chat"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();
// 5. MCP Tool Registrations
builder.Services.AddScoped<UserProfileMcpTool>();
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // e.g., 100 MB for short videos
});
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
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

// 8. Middleware Pipeline Setup
app.UseExceptionHandler();
app.UseCors("AllowNextJs");


app.MapOpenApi();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "UniConnect.Api v1");
});


// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// 9. Endpoint Mappings
app.MapGroup("/api/identity")
   .MapIdentityApi<ApplicationUser>()
   .WithTags("Auth")
   .AllowAnonymous();

app.MapMcp("/mcp");
app.MapHub<ChatHub>("/hubs/chat");
app.MapControllers();

app.Urls.Add("http://0.0.0.0:5116");
app.Run();
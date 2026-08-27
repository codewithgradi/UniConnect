using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DevDB");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Add Identity
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddInfrastructureRepositories().AddApplicationServices();
builder.Services.AddControllers();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// await DatabaseSeeder.SeedAsync(app.Services);

app.UseAuthentication();
app.UseAuthorization();
app.MapGroup("/api/identity").MapIdentityApi<ApplicationUser>().WithTags("Auth");
app.MapControllers();

app.Run();
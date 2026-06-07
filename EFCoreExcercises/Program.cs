using EFCoreExcercises.Data;
using EFCoreExcercises.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Database provider selection -------------------------------------------
// Driven by the "DatabaseProvider" config key so we can switch from
// PostgreSQL to SQL Server later WITHOUT touching code (just config).
// Valid values: "Postgres" (default) | "SqlServer".
var provider = builder.Configuration.GetValue<string>("DatabaseProvider") ?? "Postgres";
var connectionString = builder.Configuration.GetConnectionString(provider)
    ?? throw new InvalidOperationException(
        $"No connection string found for provider '{provider}'. " +
        $"Add ConnectionStrings:{provider} to configuration.");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    switch (provider.ToLowerInvariant())
    {
        case "sqlserver":
            options.UseSqlServer(connectionString);
            break;
        case "postgres":
        case "postgresql":
            options.UseNpgsql(connectionString);
            break;
        default:
            throw new InvalidOperationException($"Unsupported DatabaseProvider '{provider}'.");
    }
});

// Application services
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

// Apply pending migrations on startup (handy for containers / local dev).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

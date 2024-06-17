using MyPosTask.Infrastructure.Persistence;
using MyPosTask.Infrastructure.Data.Interceptors;
using Serilog;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Application.Common.Mappings;
using MyPosTask.Application.Person.Commands.CreatePerson;
using MyPosTask.Web;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddKeyVaultIfConfigured(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddMemoryCache();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

// Register TimeProvider
builder.Services.AddSingleton<TimeProvider, SystemTimeProvider>();

// Register AuditableEntityInterceptor
builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

// Register IUser (assuming you have a User implementation)
builder.Services.AddScoped<IUser, UserImplementation>();

// Add Health Checks
builder.Services.AddHealthChecks();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreatePersonCommand>());

builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
builder.Services.AddAutoMapper(typeof(MappingProfile));

var app = builder.Build();

app.UseHealthChecks("/health");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapFallbackToFile("index.html");
app.UseExceptionHandler(options => { });
app.Map("/", () => Results.Redirect("/api"));
app.MapEndpoints();

app.Run();

namespace MyPosTask.Web
{
    public partial class Program { }
}

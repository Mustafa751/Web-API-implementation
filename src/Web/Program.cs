using FluentValidation;
using MyPosTask.Infrastructure.Persistence;
using MyPosTask.Infrastructure.Data.Interceptors;
using Serilog;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection.People.Commands;
using Microsoft.Extensions.DependencyInjection.People.Commands.UpdatePerson;
using Microsoft.Extensions.DependencyInjection.Person.Commands.DeletePerson;
using MyPosTask.Application.Common.Interfaces;
using MyPosTask.Application.Common.Mappings;

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

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddScoped<IApplicationDbContext>(provider => (IApplicationDbContext)provider.GetRequiredService<ApplicationDbContext>());

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
builder.Services.AddValidatorsFromAssemblyContaining<CreatePersonCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdatePersonCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DeletePersonCommandValidator>();
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

public partial class Program { }

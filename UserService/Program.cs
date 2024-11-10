using Serilog;
using Microsoft.EntityFrameworkCore;
using UserService.Database;
using UserService.Database.Models;
using UserService.Utils;
using UserService.Repositories;
using UserService.Services.Account;
using UserService.Services.Profile;

var builder = WebApplication.CreateBuilder(args);

Logging.configureLogging();

builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IProfileService, ProfileService>();


var app = builder.Build();

app.UseHttpsRedirection();

app.Run();
using System.Net;
using FileExplorer.Data;
using FileExplorer.Data.Extensions;
using FileExplorer.Endpoints;
using FileExplorer.Models;
using FileExplorer.Services;
using FileExplorer.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host
    .UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration))
    .UseDefaultServiceProvider(options =>
    {
        options.ValidateOnBuild = true;
        options.ValidateScopes = true;
    });

var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<FilesDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    options.EnableSensitiveDataLogging();
    options.EnableDetailedErrors();
});

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireUppercase = false;
        options.Password.RequireDigit = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddEntityFrameworkStores<FilesDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        return Task.CompletedTask;
    };
});

builder.Services.AddHostedService<BackgroundTaskHandler>();
builder.Services.AddSingleton<IUploadProgressService, UploadProgressService>();
builder.Services.AddScoped<IValidator<LogIn>, LogInValidator>();
builder.Services.AddScoped<IUserFileService, UserFileService>();
builder.Services.AddScoped<IFileShareLinkService, FileShareLinkService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.DatabaseMigrateAsync<FilesDbContext>();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityEndpoints();
app.MapFileEndpoints();
app.MapOtherEndpoints();

app.Run();
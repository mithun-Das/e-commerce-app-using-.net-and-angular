using API.Extensions;
using API.Helpers;
using API.Middleware;
using Core.Entities.Identity;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationService();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddDbContext<StoreContext>
    (x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContext<AppIdentityDbContext>
    (x => x.UseSqlite(builder.Configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});
builder.Services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions
                        .Parse(builder.Configuration.GetConnectionString("Redis"), true);

    return ConnectionMultiplexer.Connect(configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwaggerDocumentation();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
    RequestPath = "/Content"
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToController("Index", "Fallback");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context, loggerFactory);

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        await identityContext.Database.MigrateAsync();
        await AppIdentityDbContextSeed.SeedUsersAsync(userManager);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration");
    }
};

app.Run();

using API.Extensions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationService();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddDbContext<StoreContext>
    (x => x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(MappingProfiles));
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseMiddleware<ExceptionMiddleware>();

app.UseSwaggerDocumentation();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    
    try
    {
        var context = services.GetRequiredService<StoreContext>();
        await context.Database.MigrateAsync();
        await StoreContextSeed.SeedAsync(context, loggerFactory);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration");
    }
};

app.Run();

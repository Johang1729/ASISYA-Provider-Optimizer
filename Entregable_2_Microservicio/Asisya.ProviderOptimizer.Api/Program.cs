using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Asisya.ProviderOptimizer.Infrastructure.Persistence;
using Asisya.ProviderOptimizer.Application.Interfaces;
using Asisya.ProviderOptimizer.Infrastructure.Repositories;
using Asisya.ProviderOptimizer.Application.Commands;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=AsisyaOptimizer;Username=postgres;Password=admin";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(defaultConnection)
);

builder.Services.AddScoped<IProviderRepository, ProviderRepository>();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(OptimizeAssignmentCommand).Assembly);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Optimizer API v1");
    c.RoutePrefix = string.Empty; 
});

app.UseAuthorization();
app.MapControllers();

app.Run();

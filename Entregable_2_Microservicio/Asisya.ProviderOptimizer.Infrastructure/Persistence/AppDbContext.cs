using System;
using Asisya.ProviderOptimizer.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Asisya.ProviderOptimizer.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Provider> Providers { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Provider>().HasKey(p => p.Id);

        modelBuilder.Entity<Provider>().HasData(
            new Provider { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Grúas Express VIP", VehicleCapacity = "Sedan,SUV,Moto", IncidentCapacity = "Remolque,Bateria", Rating = 4.9, AcceptanceRate = 0.95, CurrentLat = 40.7128, CurrentLng = -74.0060, Status = "Libre" },
            new Provider { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Grúas Lentas", VehicleCapacity = "Sedan,SUV", IncidentCapacity = "Remolque", Rating = 3.5, AcceptanceRate = 0.20, CurrentLat = 40.7135, CurrentLng = -74.0050, Status = "Libre" },
            new Provider { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "Cerrajero Rápido", VehicleCapacity = "Moto,Sedan", IncidentCapacity = "Llave", Rating = 4.2, AcceptanceRate = 0.85, CurrentLat = 40.7150, CurrentLng = -74.0100, Status = "Libre" },
            new Provider { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Pesados TruckTowing", VehicleCapacity = "Camion,SUV", IncidentCapacity = "Remolque", Rating = 4.8, AcceptanceRate = 0.90, CurrentLat = 40.7200, CurrentLng = -73.9900, Status = "Libre" },
            new Provider { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "El Auxilio Básico", VehicleCapacity = "Sedan", IncidentCapacity = "Remolque,Bateria", Rating = 4.0, AcceptanceRate = 0.70, CurrentLat = 40.7180, CurrentLng = -74.0010, Status = "Libre" },
            new Provider { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Grúa Ocupada", VehicleCapacity = "Sedan,SUV", IncidentCapacity = "Remolque", Rating = 5.0, AcceptanceRate = 1.0, CurrentLat = 40.7120, CurrentLng = -74.0050, Status = "Ocupado" }
        );
    }
}

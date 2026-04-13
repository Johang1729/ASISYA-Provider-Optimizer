using System;
using System.Linq;
using System.Threading.Tasks;
using Asisya.ProviderOptimizer.Domain.Entities;
using Asisya.ProviderOptimizer.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Asisya.ProviderOptimizer.Tests;

public class OptimizerIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public OptimizerIntegrationTests()
    {
        _dbContainer = new PostgreSqlBuilder("postgres:15-alpine")
            .WithDatabase("test_asisya")
            .WithUsername("postgres")
            .WithPassword("test_password")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task Database_SeedData_ContainsExpectedProviders()
    {
        using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        var providers = await context.Providers.ToListAsync();

        providers.Should().NotBeEmpty();
        providers.Should().HaveCountGreaterThan(4);
    }

    [Fact]
    public async Task Query_AvailableProviders_ExcludesOccupied()
    {
        using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        var available = await context.Providers
            .Where(p => p.Status == "Libre")
            .ToListAsync();

        available.Should().NotBeEmpty();
        available.Should().OnlyContain(p => p.Status == "Libre");

        var occupied = await context.Providers
            .Where(p => p.Status == "Ocupado")
            .ToListAsync();

        occupied.Should().NotBeEmpty("seed data should include at least one busy provider");
    }

    [Fact]
    public async Task Provider_StatusUpdate_PersistsCorrectly()
    {
        using var context = CreateContext();
        await context.Database.EnsureCreatedAsync();

        var provider = await context.Providers.FirstAsync(p => p.Status == "Libre");
        var originalId = provider.Id;

        provider.Status = "Ocupado";
        await context.SaveChangesAsync();

        using var verifyContext = CreateContext();
        var updated = await verifyContext.Providers.FindAsync(originalId);

        updated.Should().NotBeNull();
        updated!.Status.Should().Be("Ocupado");
    }
}

using System;
using Xunit;
using FluentAssertions;
using Asisya.ProviderOptimizer.Domain.Entities;
using Asisya.ProviderOptimizer.Domain.Services;

namespace Asisya.ProviderOptimizer.Tests;

public class ScoringPolicyTests
{
    private readonly ScoringPolicy _sut; 

    public ScoringPolicyTests()
    {
        _sut = new ScoringPolicy();
    }

    [Fact]
    public void CalculateScore_WhenProviderIsBusy_Returns0()
    {
        var provider = new Provider { Status = "Ocupado", VehicleCapacity = "Sedan", IncidentCapacity = "Remolque" };
        var ticket = new OptimizationTicket { VehicleCategory = "Sedan", IncidentType = "Remolque", IsPremiumUser = false };

        var result = _sut.CalculateScore(provider, ticket, 5.0, 15.0);

        result.Should().Be(0); 
    }

    [Fact]
    public void CalculateScore_WhenCapacityDoesNotMatch_Returns0()
    {
        var provider = new Provider { Status = "Libre", VehicleCapacity = "Moto", IncidentCapacity = "Remolque" };
        var ticket = new OptimizationTicket { VehicleCategory = "Sedan", IncidentType = "Remolque" };

        var result = _sut.CalculateScore(provider, ticket, 5.0, 15.0);

        result.Should().Be(0); 
    }

    [Fact]
    public void CalculateScore_WhenPremiumUser_BoostsScore()
    {
        var provider = new Provider { Status = "Libre", VehicleCapacity = "Sedan", IncidentCapacity = "Remolque", Rating = 5.0, AcceptanceRate = 1.0 };
        var normalTicket = new OptimizationTicket { VehicleCategory = "Sedan", IncidentType = "Remolque", IsPremiumUser = false };
        var premiumTicket = new OptimizationTicket { VehicleCategory = "Sedan", IncidentType = "Remolque", IsPremiumUser = true };

        var normalScore = _sut.CalculateScore(provider, normalTicket, 2.0, 5.0);
        var premiumScore = _sut.CalculateScore(provider, premiumTicket, 2.0, 5.0);

        premiumScore.Should().BeGreaterThan(normalScore);
    }
}
